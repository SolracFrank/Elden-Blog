using Application.Features.Auth.ValidateSession;
using Application.Interfaces.AuthServices;
using Domain.Dtos.Auth;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.AuthServices
{
    public class ValidateSessionService : IValidateSessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ValidateSessionService> _logger;

        public ValidateSessionService(IUnitOfWork unitOfWork, ILogger<ValidateSessionService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> ValidateSession(ValidateSessionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Validating request");
            if (request.UserId == null || request.RefreshToken == null)
            {
                _logger.LogInformation("Token invalid");

                return false;          
            }

            _logger.LogInformation("Validating stored refreshToken");
            var StoredRefreshToken = await _unitOfWork.RefreshTokens.FirstOrDefaultAsync(x => x.Token == request.RefreshToken
            && x.UserId == request.UserId && x.IsExpired == false && x.Revoked == null, cancellationToken);

            if (StoredRefreshToken == null)
            {
                _logger.LogInformation("Token invalid");

                return false;
            }
            _logger.LogInformation("Token valid");

            return true;
        }

    }
}
