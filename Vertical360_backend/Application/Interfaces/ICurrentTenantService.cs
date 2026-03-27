namespace Vertical360_backend.Application.Interfaces
{
    public interface ICurrentTenantService
    {
        string? TenantId { get; }
        Task SetTenantAsync(string tenantId);
    }
}
