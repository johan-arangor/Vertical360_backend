using Vertical360_back.Domain.Interfaces;
using Vertical360_back.Infrastructure.Persistence;
using Vertical360_back.Infrastructure.Repositories;

namespace Vertical360_back.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IResidentRepository, ResidentRepository>();

            return services;
        }
    }
}
