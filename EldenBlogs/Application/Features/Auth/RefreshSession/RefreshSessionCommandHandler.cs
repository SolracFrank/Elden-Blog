using Application.Interfaces.AuthServices;
using Domain.Dtos.Token;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.RefreshSession
{
    public class RefreshSessionCommandHandler : IRequestHandler<RefreshSessionCommand, Result<JWTResult>>
    {
        private readonly ILogger<RefreshSessionCommandHandler> _logger;
        private readonly IAuthService _authService;

        public RefreshSessionCommandHandler(ILogger<RefreshSessionCommandHandler> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        public async Task<Result<JWTResult>> Handle(RefreshSessionCommand request, CancellationToken cancellationToken)
        {
            var validation = new RefreshSessionCommandValidator();
            var validationResult = await validation.ValidateAsync(request, cancellationToken);

            #region Validation Behavior
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation errors with Refresh token");

                return new Result<JWTResult>(new FluentValidation.ValidationException(validationResult.Errors));
            }
            #endregion

            var result = await _authService.RefreshSessionToken(request, cancellationToken);
            if(result == null)
            {
                return new Result<JWTResult>(new ApplicationException("An error has ocurred while "));
            }
            return new Result<JWTResult>(result);
        }
    }
}
