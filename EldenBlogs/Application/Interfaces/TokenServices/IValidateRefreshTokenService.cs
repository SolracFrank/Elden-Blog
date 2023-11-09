using Domain.Dtos.Token;

namespace Application.Interfaces.TokenServices
{
    public interface IValidateRefreshTokenService
    {
        public Task<RefreshToken> ValidateRefreshToken(string tokenToValidate, string userId, string ip, CancellationToken cancellationToken);
    }
}
