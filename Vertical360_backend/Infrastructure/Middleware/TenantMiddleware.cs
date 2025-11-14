using Vertical360_backend.Application.Interfaces;
using Vertical360_backend.Domain.ValueObjects;

namespace Vertical360_backend.Infrastructure.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        public TenantMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, ICurrentTenantService tenantService)
        {
            string? tenant = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(tenant) && context.User.Identity?.IsAuthenticated == true)
            {
                tenant = context.User.Claims.FirstOrDefault(c => c.Type == Constants.CLAIM_TENANT)?.Value;
            }
            if (!string.IsNullOrEmpty(tenant))
            {
                await tenantService.SetTenantAsync(tenant);
            }
            await _next(context);
        }
    }
}
