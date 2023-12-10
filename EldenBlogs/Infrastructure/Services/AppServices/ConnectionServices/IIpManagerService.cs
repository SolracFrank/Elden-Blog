using Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.AppServices.ConnectionServices
{
    public class IpManagerService : IIpManagerService
    {
        private readonly IHttpContextAccessor _httpAccesor;

        public IpManagerService(IHttpContextAccessor httpAccesor)
        {
            _httpAccesor = httpAccesor;
        }

        public string GenerateIpAddress()
        {
            var httpContext = _httpAccesor.HttpContext;

            if (httpContext == null)
            {
                throw new ApiException ("IP Unavailable");
            }

            if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            }
            else if (httpContext.Connection.RemoteIpAddress != null)
            {
                return httpContext.Connection.RemoteIpAddress.ToString();
            }
            throw new ApiException("IP Unavailable");
        }
    }
}

