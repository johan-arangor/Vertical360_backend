using Microsoft.AspNetCore.Http;
using Vertical360.Application.Interfaces;
using Vertical360.Core.ValueObjects;
using System.Security.Claims;

namespace Vertical360.Infrastructure.Middleware
{
    /// <summary>
    /// Middleware encargado de resolver el TenantId a partir de los Claims del JWT
    /// en cada petición autenticada.
    /// </summary>
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICurrentTenantService tenantService)
        {
            // Solo procesamos el tenant si el usuario ya está autenticado (middleware de JWT previo)
            if (context.User.Identity?.IsAuthenticated == true)
            {
                // Extraer el tenant_id desde los claims del JWT
                var tenantId = context.User.Claims
                    .FirstOrDefault(c => c.Type == Constants.CLAIM_TENANT || c.Type == "tenant_id")?.Value;

                if (string.IsNullOrEmpty(tenantId))
                {
                    // Si el usuario está autenticado pero no tiene tenant_id, la sesión es inválida
                    // para el contexto multi-tenant de este sistema.
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new 
                    { 
                        error = "Acceso denegado. El token de seguridad no contiene la identificación de la copropiedad (tenant_id)." 
                    });
                    return;
                }

                // Inyectar el tenantId en el servicio de scoped para que esté disponible en toda la petición
                await tenantService.SetTenantAsync(tenantId);
            }

            await _next(context);
        }
    }
}
