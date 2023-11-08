using Application.Features.Auth.Register;

namespace Application.Interfaces.AuthServices
{
    public interface IAuthService
    {
        public Task<string> RegisterAsync(RegisterCommand registerRequest, CancellationToken cancellationToken);

    }
}
