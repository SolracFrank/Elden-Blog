using Application.Features.Auth.Login;
using Application.Interfaces.AuthServices;
using Domain.Dtos.Auth;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.ValidateSession
{
    public class ValidateSessionCommandHandler : IRequestHandler<ValidateSessionCommand, Result<SessionValidationResult>>
    {
        private readonly ILogger<LoginCommandHandler> _logger;
        private readonly IValidateSessionService _validateSession;

        public ValidateSessionCommandHandler(ILogger<LoginCommandHandler> logger, IValidateSessionService validateSession)
        {
            _logger = logger;
            _validateSession = validateSession;
        }

        public async Task<Result<SessionValidationResult>> Handle(ValidateSessionCommand request, CancellationToken cancellationToken)
        {
            var isSessionValidated = await _validateSession.ValidateSession(new ValidateSessionCommand { UserId = request.UserId, RefreshToken = request.RefreshToken }, cancellationToken);

            var sessionValidationResult = new SessionValidationResult
            {
                IsValidated = isSessionValidated
            };

            return new Result<SessionValidationResult>(sessionValidationResult);
        }
    }
}
