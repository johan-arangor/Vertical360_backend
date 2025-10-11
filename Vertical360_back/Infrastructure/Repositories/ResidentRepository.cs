using Microsoft.EntityFrameworkCore;
using Vertical360_back.Domain.Entityes.Tenants;
using Vertical360_back.Domain.Interfaces;
using Vertical360_back.Infrastructure.Persistence;

namespace Vertical360_back.Infrastructure.Repositories
{
    public class ResidentRepository : EfRepository<Resident>, IResidentRepository
    {
        public ResidentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Resident?> GetByDocumentAsync(string document)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.Document == document);
        }
    }

    public interface IResidentRepository : IRepository<Resident>
    {
        Task<Resident?> GetByDocumentAsync(string document);
    }
}
