namespace Vertical360.Application.Interfaces
{
    public interface ICurrentTenantService
    {
        string? TenantId { get; }
        Task SetTenantAsync(string tenantId);
    }
}
