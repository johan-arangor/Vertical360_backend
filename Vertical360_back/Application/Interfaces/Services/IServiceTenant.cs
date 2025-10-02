namespace Vertical360_back.Application.Interfaces.Services
{
    public interface IServiceTenant
    {
        // Devuelve el nombre de la BD del cliente (Tenant DB Name)
        string GetTenantConnectionString();
    }
}
