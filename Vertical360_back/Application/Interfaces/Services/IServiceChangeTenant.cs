namespace Vertical360_back.Application.Interfaces.Services
{
    public interface IServiceChangeTenant
    {
        Task ReplaceTenant(Guid companyId, string newTenantId);
    }
}
