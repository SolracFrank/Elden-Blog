using Application.Interfaces.AuthServices;
using Domain.Dtos.Token;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<JWTResult>>
    {
        private readonly ILogger<LoginCommandHandler> _logger;
        private readonly IAuthService _authService;
        public LoginCommandHandler(ILogger<LoginCommandHandler> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }
        public async Task<Result<JWTResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var validation = new LoginCommandValidator();
            var validationResult = await validation.ValidateAsync(request, cancellationToken);

            #region Validation Behavior
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation errors with Todo");

                return new Result<JWTResult>(new FluentValidation.ValidationException(validationResult.Errors));
            }
            #endregion

           var result = await _authService.LoginAsync(request, cancellationToken);
            return new Result<JWTResult>(result);
        }
    }
}
