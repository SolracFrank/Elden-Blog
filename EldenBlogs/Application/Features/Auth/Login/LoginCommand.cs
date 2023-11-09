using Domain.Dtos.Token;
using LanguageExt.Common;
using MediatR;

namespace Application.Features.Auth.Login
{
    public class LoginCommand : IRequest<Result<JWTResult>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Ip { get; set; }
    }
}
