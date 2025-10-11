namespace Vertical360_back.Application.Interfaces.Services
{
    public interface ICurrentTenantService
    {
        string TenantId { get; }
        string? UserId { get; }
        bool HasTenant { get; }
    }
}
