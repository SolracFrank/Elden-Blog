using Domain.Dtos.Auth;
using LanguageExt.Common;
using MediatR;

namespace Application.Features.Auth.ValidateSession
{
    public class ValidateSessionCommand : IRequest<Result<SessionValidationResult>>
    {
        public string UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}
