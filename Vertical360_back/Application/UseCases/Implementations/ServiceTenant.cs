using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Vertical360_back.Application.Interfaces.Services;
using Vertical360_back.Domain.ValueObjects;
using Vertical360_back.Infrastructure.Persistence;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Vertical360_back.Application.UseCases.Implementations
{
    public class ServiceTenant : IServiceTenant
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CommonDbContext _commonDbContext;

        public ServiceTenant(IHttpContextAccessor httpContextAccessor, CommonDbContext commonDbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _commonDbContext = commonDbContext;
        }

        public string GetTenantConnectionString()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is null)
            {
                // Devuelve la cadena vacía si no hay contexto
                return string.Empty;
            }

            // Obtener el GUID del Cliente Seleccionado del usuario autenticado
            // Asumimos que el ClaimType para el Tenant ID es 'tenant_id' o similar.
            var claimTenant = httpContext.User.Claims
                .FirstOrDefault(c => c.Type == "tenant_id"); // Reemplaza "tenant_id" con tu constante

            if (claimTenant is null || !Guid.TryParse(claimTenant.Value, out var companyId))
            {
                return string.Empty;
            }

            // Consultar la Base de Datos Común para obtener la cadena de conexión.
            var company = _commonDbContext.Companies
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == companyId);

            // Devolver la cadena de conexión o cadena vacía si no se encuentra.
            return company?.ConnectionString ?? string.Empty;
        }
    }
}
