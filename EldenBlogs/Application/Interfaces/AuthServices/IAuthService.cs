using Application.Features.Auth.Login;
using Application.Features.Auth.RefreshSession;
using Application.Features.Auth.Register;
using Domain.Dtos.Token;

namespace Application.Interfaces.AuthServices
{
    public interface IAuthService
    {
        public Task<string> RegisterAsync(RegisterCommand registerRequest, CancellationToken cancellationToken);
        public Task<JWTResult> LoginAsync(LoginCommand loginRequest, CancellationToken cancellationToken);
        public Task<JWTResult> RefreshSessionToken(RefreshSessionCommand refreshRequest, CancellationToken cancellationToken);

    }
}
