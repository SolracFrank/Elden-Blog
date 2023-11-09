using Application.Interfaces.TokenServices;
using Domain.Dtos.Token;
using Domain.Exceptions;
using Infrastructure.CustomEntities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services.TokenServices
{
    public class GenerateJWTService<T> : IGenerateJWTService<T> where T : User
    {
        private readonly JWTSettings _jwtSettings;
        private readonly ILogger<GenerateJWTService<T>> _logger;

        public GenerateJWTService(ILogger<GenerateJWTService<T>> logger, IOptions<JWTSettings> jwtSettings)
        {
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
        }
        
        public  JWTResult GenerateJWTToken(T user, ClaimsIdentity claimsIdentity)
        {
            if(claimsIdentity == null)
            {
                _logger.LogInformation($"User {user.UserName} - {user.Email} doesn't have claims ");
                throw new ValidationException("User doesn't have claims");
            }

            foreach (var property in typeof(JWTSettings).GetProperties())
            {
                var value = property.GetValue(_jwtSettings, null);
                if (value == null || (value is string && string.IsNullOrEmpty((string)value)))
                {
                    _logger.LogCritical($"JWTSettings property '{property.Name}' is not properly configured");
                    throw new InfrastructureException($"JWTSettings property '{property.Name}' hasn't been configured");
                }
            }

            var expirationDate = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes);

            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signInCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
               issuer: _jwtSettings.Issuer,
               audience: _jwtSettings.Audience,
               claims: claimsIdentity.Claims,
               expires: expirationDate,
               signingCredentials: signInCredentials
               );

            var tokenResult = new JWTResult
            {
                Email = user.Email,
                UserName = user.UserName,
                UserId = user.Id,
                JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                JWTExpires = expirationDate,
            };

            return tokenResult;
        }
    }
}
