using Application.Features.Auth.Register;

namespace Application.Interfaces
{
    public interface IRegistationService
    {
        public Task<string> RegisterAsync(RegisterCommand registerRequest, CancellationToken cancellationToken);

    }
}
