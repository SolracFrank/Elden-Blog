using Domain.Dtos.Token;
using LanguageExt.Common;
using MediatR;

namespace Application.Features.Auth.RefreshSession
{
    public class RefreshSessionCommand : IRequest<Result<JWTResult>>
    {
        public required string UserId { get; set; }
        public required string IpAddress { get; set; }
        public string RefreshToken { get; set; }
    }
}
