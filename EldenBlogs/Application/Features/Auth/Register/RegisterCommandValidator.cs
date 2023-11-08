using FluentValidation;

namespace Application.Features.Auth.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email");

            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Password required")
                .MinimumLength(6).WithMessage("Password minimux length is 6")
                .Matches("(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d_@#$%]{6,}$").WithMessage("Password must contain atleast one upercase, and one number");

            RuleFor(r => r.RepeatPassword)
                 .NotEmpty().WithMessage("Repeat password required")
                 .Equal(r => r.Password).WithMessage("Passwords must match");

            RuleFor(r => r.Birthday)
               .NotEmpty().WithMessage("Birthdate is required")
               .Must(BeAValidDate).When(r => r.Birthday != null)
               .WithMessage("Date format is not valid");

            RuleFor(r => r.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(6).WithMessage("Username minimum length is 6");

        }
        private bool BeAValidDate(DateTime date)
        {
            return date >= DateTime.MinValue && date <= DateTime.MaxValue;
        }

    }
}
