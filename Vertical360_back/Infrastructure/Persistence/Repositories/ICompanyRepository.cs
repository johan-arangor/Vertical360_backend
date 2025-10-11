using System.Linq.Expressions;
using Vertical360_back.Domain.Entityes.Common;

namespace Vertical360_back.Infrastructure.Persistence.Repositories
{
    public interface ICompanyRepository
    {
        Task<Companies?> FirstOrDefaultAsync(Expression<Func<Companies, bool>> predicate);
        Task<Companies> AddAsync(Companies company);
        Task<List<Companies>> GetAllAsync();
        Task<Companies?> GetByIdAsync(Guid id);
        Task UpdateAsync(Companies company);
        Task DeleteAsync(Guid id);
        Task<LinkUserCompany> AddLinkAsync(LinkUserCompany link);
        Task AddLinksAsync(IEnumerable<LinkUserCompany> links);
    }
}
