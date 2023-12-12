using Application.enums;
using Application.Features.Auth.Login;
using Application.Features.Auth.RefreshSession;
using Application.Features.Auth.Register;
using Application.Interfaces.AuthServices;
using Application.Interfaces.TokenServices;
using Azure;
using Domain.Dtos.Token;
using Domain.Exceptions;
using Domain.Interfaces;
using Infrastructure.CustomEntities;
using Infrastructure.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.Services.AuthServices
{
    internal class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AuthService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenerateJWTService<User> _generateJWTService;
        private readonly IValidateRefreshTokenService _validateRefreshTokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidateUserService<User> _validateUserService;

        public AuthService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger<AuthService> logger, IUnitOfWork unitOfWork, SignInManager<User> signInManager, IGenerateJWTService<User> generateJWTService, IHttpContextAccessor httpContextAccessor, IValidateRefreshTokenService validateRefreshTokenService, IValidateUserService<User> validateUserService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _signInManager = signInManager;
            _generateJWTService = generateJWTService;
            _httpContextAccessor = httpContextAccessor;
            _validateRefreshTokenService = validateRefreshTokenService;
            _validateUserService = validateUserService;
        }

        public async Task<string> RegisterAsync(RegisterCommand registerRequest, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(registerRequest.Email);

            user.EnsureDoesNotExist(_logger);

            if (!await _roleManager.RoleExistsAsync(Roles.Poster.ToString()))
            {
                await _roleManager.CreateAsync(new IdentityRole(Roles.Poster.ToString()));
            }

            user = new User
            {
                BirthDate = registerRequest.Birthday,
                Email = registerRequest.Email,
                UserName = registerRequest.Username,
            };

            return await _unitOfWork.ExecuteWithinTransactionAsync(async () =>
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var result = await _userManager.CreateAsync(user, registerRequest.Password);

                    _logger.LogInformation("Creating User");

                    if (!result.Succeeded)
                    {
                        _logger.LogInformation($"Error on creating User: {result.Errors}");
                        throw new ApiException($"One or more errors has ocurred while creating user: {result.Errors}");
                    }

                    await _userManager.AddToRoleAsync(user, Roles.Poster.ToString());

                    var userClaims = new List<Claim>
                    {
                       new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                       new Claim(JwtRegisteredClaimNames.Email, user.Email),
                       new Claim("Active","True"),
                    };

                    await _userManager.AddClaimsAsync(user, userClaims);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    await _unitOfWork.CommitTransactionAsync(cancellationToken);
                    return $"User {user.UserName} has been registered succesfully";
                }
                catch (ApiException ex)
                {
                     _unitOfWork.RollbackTransaction();
                    _logger.LogError(ex, "Un error ocurrió durante el registro");
                    throw new ApiException($"Ocurrió un error: {ex.Message}");
                }
            });
        }

        public async Task<JWTResult> LoginAsync(LoginCommand loginRequest, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Searching user...");
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);

            user.EnsureExists(_logger, "User not found", "Wrong credentials");

            _logger.LogInformation("Validating Password");

            var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, true, true);
            if (!result.Succeeded)
            {
                _logger.LogInformation("Wrong password");
                throw new BadRequestException("Wrong credentials");
            }

            var claimIdentity = await _validateUserService.ValidateUserClaims(user);

            _logger.LogInformation("Creating JWT Token...");
            JWTResult jwtResult = _generateJWTService.GenerateJWTToken(user, claimIdentity);
            _logger.LogInformation("JWT Token created.");

            var refreshToken = new RefreshToken
            {
                Id = new Guid(),
                UserId = user.Id,
                CreatedByIp = loginRequest.Ip,
                Expires = DateTime.UtcNow.AddDays(30),
                Token = RefreshTokenGenerator.RandomTokenString(),
            };
            _logger.LogInformation("Creating refreshToken.");

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            _logger.LogInformation("Saving RefreshToken in Database...");
            var refreshTokenResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if(!refreshTokenResult)
            {
                _logger.LogInformation("Failed to save RefreshToken");
                throw new ApiException("Error on login, try again");
            }
            jwtResult.SessionDuration = jwtResult.SessionDuration;
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expires,
                SameSite = SameSiteMode.Strict,
                Secure = true 
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

            return jwtResult;
        }

        public async Task<JWTResult> RefreshSessionToken(RefreshSessionCommand refreshRequest, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Validating user...");

            var user = await _userManager.FindByIdAsync(refreshRequest.UserId);
            user.EnsureExists(_logger, "User not found", "There's a problem with the session");
            
            var newRefreshToken = await _validateRefreshTokenService.ValidateRefreshToken(refreshRequest.RefreshToken, refreshRequest.UserId, refreshRequest.IpAddress, cancellationToken);

            if(newRefreshToken == null)
            {
                throw new ValidationException("Error on genereting new refresh token");
            }

            var claimIdentity = await _validateUserService.ValidateUserClaims(user);

            _logger.LogInformation("Creating JWT Token...");
            JWTResult jwtResult = _generateJWTService.GenerateJWTToken(user, claimIdentity);
            jwtResult.SessionDuration = newRefreshToken.Expires;
            _logger.LogInformation("JWT Token created.");


            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires,
                SameSite = SameSiteMode.Strict,
                Secure = true,
                
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            return jwtResult;
        }
    }
}
