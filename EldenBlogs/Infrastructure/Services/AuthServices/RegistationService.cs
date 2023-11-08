using Application.enums;
using Application.Features.Auth.Register;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using Infrastructure.CustomEntities;
using Infrastructure.Repositories;
using Infrastructure.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure.Services.AuthServices
{
    internal class RegistationService : IRegistationService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<RegistationService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public RegistationService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger<RegistationService> logger, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _unitOfWork = unitOfWork;
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
                   new Claim("ip", registerRequest.IpAddress),
                   new Claim("Active","True")
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

    }
}
