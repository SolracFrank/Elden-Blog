using Domain.Dtos.Token;
using System.Security.Claims;

namespace Application.Interfaces.TokenServices
{
    public interface IGenerateJWTService<T> where T : class
    {
        public JWTResult GenerateJWTToken(T user, ClaimsIdentity claimsIdentity);
    }
}
