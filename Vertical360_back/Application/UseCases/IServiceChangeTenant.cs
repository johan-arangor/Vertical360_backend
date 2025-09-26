namespace Vertical360_back.Services
{
    public interface IServiceChangeTenant
    {
        Task ReplaceTenant(Guid companyId, string newTenantId);
    }
}
