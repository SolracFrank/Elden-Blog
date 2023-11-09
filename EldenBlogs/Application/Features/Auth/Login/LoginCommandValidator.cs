using FluentValidation;


namespace Application.Features.Auth.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .EmailAddress().WithMessage("{PropertyName} is invalid");


            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("{PropertyName} is required");
        }
    }

    
    
}
