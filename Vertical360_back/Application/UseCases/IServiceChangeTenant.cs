namespace Vertical360_back.Application.UseCases
{
    public interface IServiceChangeTenant
    {
        Task ReplaceTenant(Guid companyId, string newTenantId);
    }
}
