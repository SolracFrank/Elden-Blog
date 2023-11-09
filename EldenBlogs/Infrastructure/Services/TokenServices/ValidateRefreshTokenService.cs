using Domain.Dtos.Token;
using Domain.Exceptions;
using Domain.Interfaces;
using Infrastructure.Utils;
using LanguageExt;
using Microsoft.Extensions.Logging;

namespace Application.Interfaces.TokenServices
{
    public class ValidateRefreshTokenService : IValidateRefreshTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ValidateRefreshTokenService> _logger;

        public ValidateRefreshTokenService(IUnitOfWork unitOfWork, ILogger<ValidateRefreshTokenService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<RefreshToken> ValidateRefreshToken(string tokenToValidate, string userId, string ip, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Validation refreshToken.");
            if (tokenToValidate.IsNull() || tokenToValidate == "")
            {
                _logger.LogInformation("Token is null");
                throw new ValidationException("Session token doesn't exist");
            }
            _logger.LogInformation("Getting stored RefreshToken.");
            var refreshToken = await _unitOfWork.RefreshTokens.FirstOrDefaultAsync(t => t.Token == tokenToValidate && t.UserId == userId
          , cancellationToken);

            if (refreshToken == null)
            {
                _logger.LogInformation("StoredToken is invalid.");
                throw new ValidationException("Session doesn't exist");
            }
            if(refreshToken.IsExpired)
            {
                _logger.LogInformation("StoredToken is expired.");
                throw new ValidationException("Session expired");
            }
            if (refreshToken.Revoked != null)
            {
                _logger.LogInformation("StoredToken is revoked.");
                throw new ValidationException("Session revoked");
            }


            var newRefreshToken = new RefreshToken
            {
                Id = new Guid(),
                UserId = userId,
                CreatedByIp = ip,
                Expires = DateTime.UtcNow.AddDays(30),
                Token = RefreshTokenGenerator.RandomTokenString(),
            };
            refreshToken.TokenReplaced = newRefreshToken.Token;
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ip;

            _unitOfWork.RefreshTokens.Update(refreshToken);
            await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);

            _logger.LogInformation("Saving new RefreshToken in Database...");
            var refreshTokenResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (!refreshTokenResult)
            {
                _logger.LogInformation("Failed to save new RefreshToken");
                throw new ApiException("Error on refreshing token");
            }

            return newRefreshToken;
        }
    }
}
