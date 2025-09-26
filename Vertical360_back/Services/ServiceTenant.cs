using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Vertical360_back.Application.UseCases;

namespace Vertical360_back.Services
{
    public class ServiceTenant : IServiceTenant
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServiceTenant(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public string GetTenant()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is null)
            {
                return string.Empty;
            }

            var authTicket = DecryptAuthCookie(httpContext);

            if (authTicket is null)
            {
                return string.Empty;
            }

            var claimTenant = authTicket.Principal.Claims.FirstOrDefault(x => x.Type == Constants.CLAIM_TENANT);

            if (claimTenant is null)
            {
                return string.Empty;
            }

            return claimTenant.Value;
        }

        private static AuthenticationTicket? DecryptAuthCookie(HttpContext httpContext)
        {
            var option = httpContext.RequestServices
                .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
                .Get("Identity.Application");

            var cookie = option.CookieManager
                .GetRequestCookie(httpContext, option.Cookie.Name!);

            return option.TicketDataFormat.Unprotect(cookie);
        }
    }
}
