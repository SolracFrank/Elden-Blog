using Application.Features.Auth.ValidateSession;

namespace Application.Interfaces.AuthServices
{
    public interface IValidateSessionService
    {
        public Task<bool> ValidateSession(ValidateSessionCommand request, CancellationToken cancellationToken);

    }
}
