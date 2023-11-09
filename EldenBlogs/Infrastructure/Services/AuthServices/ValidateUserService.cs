using Application.enums;
using Domain.Exceptions;
using Infrastructure.CustomEntities;
using Infrastructure.Services.AuthServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Application.Interfaces.AuthServices
{
    public class ValidateUserService <T>: IValidateUserService<T> where T : User
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<ValidateUserService<T>> _logger;

        public ValidateUserService(UserManager<User> userManager, ILogger<ValidateUserService<T>> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ClaimsIdentity> ValidateUserClaims(T user)
        {
            _logger.LogInformation("Getting user roles...");
            var userRoles = await _userManager.GetRolesAsync(user);

            if (!userRoles.Any())
            {
                _logger.LogInformation("Not roles founded");
                _logger.LogInformation("Adding new roles");
                var roleResult = await _userManager.AddToRoleAsync(user, Roles.Poster.ToString());
                if (!roleResult.Succeeded)
                {
                    _logger.LogInformation($"An error has ocurred while validating roles for {user.UserName}");
                    throw new ApiException("An error has ocurred while validating user");
                }
            }
            _logger.LogInformation("Roles OK");

            var userClaims = await _userManager.GetClaimsAsync(user);

            foreach (var rol in userRoles)
            {
                userClaims.Add(new Claim(rol.ToString(), "true"));
            }

            var claimIdentity = new ClaimsIdentity(userClaims, "DefaultLogin");

            return claimIdentity;
        }
    }
}
