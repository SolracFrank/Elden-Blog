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
                return "IP Unavailable";
            }

            if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            }
            else if (httpContext.Connection.RemoteIpAddress != null)
            {
                return httpContext.Connection.RemoteIpAddress.ToString();
            }

            return "IP Unavailable";
        }
    }
}

