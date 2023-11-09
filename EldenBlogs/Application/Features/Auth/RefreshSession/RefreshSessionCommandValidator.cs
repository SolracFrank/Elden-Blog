using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.RefreshSession
{
    public class RefreshSessionCommandValidator : AbstractValidator<RefreshSessionCommand>
    {
        public RefreshSessionCommandValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(r => r.UserId)
                .NotEmpty().WithMessage("{PropertyName} should not be null");
        }
    }
}
