using LanguageExt.Pipes;
using System.Security.Claims;

namespace Application.Interfaces.AuthServices
{
    public interface IValidateUserService <T> where T : class
    {
        public  Task<ClaimsIdentity> ValidateUserClaims(T user);

    }
}
