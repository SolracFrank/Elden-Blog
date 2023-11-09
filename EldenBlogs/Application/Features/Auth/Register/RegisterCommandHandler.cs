using Application.Features.Auth.Register;
using Application.Interfaces.AuthServices;
using FluentValidation;
using LanguageExt.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Auth.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
    {
        private readonly ILogger<RegisterCommandHandler> _logger;
        private readonly IAuthService _registationService;

        public RegisterCommandHandler(ILogger<RegisterCommandHandler> logger, IAuthService registationService)
        {
            _logger = logger;
            _registationService = registationService;
        }

        public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var validation = new RegisterCommandValidator();
            var validationResult = await validation.ValidateAsync(request, cancellationToken);

            #region Validation Behavior
            if (!validationResult.IsValid)
            {
                _logger.LogInformation("Validation errors with Todo");

                return new Result<string>(new ValidationException(validationResult.Errors));
            }
            #endregion
           

            var result = await _registationService.RegisterAsync(request, cancellationToken);


            return new Result<string>(result);
        }
    }
}
