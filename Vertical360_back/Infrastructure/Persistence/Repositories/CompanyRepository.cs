using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Vertical360_back.Domain.Entityes.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Vertical360_back.Infrastructure.Persistence.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Companies?> FirstOrDefaultAsync(Expression<Func<Companies, bool>> predicate)
        {
            return await _context.Companies.FirstOrDefaultAsync(predicate);
        }

        public async Task<Companies> AddAsync(Companies company)
        {
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<List<Companies>> GetAllAsync()
            => await _context.Companies.ToListAsync();

        public async Task<Companies?> GetByIdAsync(Guid id)
            => await _context.Companies.FindAsync(id);

        public async Task UpdateAsync(Companies company)
        {
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<LinkUserCompany> AddLinkAsync(LinkUserCompany link)
        {
            _context.LinkUsersCompany.Add(link);
            await _context.SaveChangesAsync();
            return link;
        }

        public async Task AddLinksAsync(IEnumerable<LinkUserCompany> links)
        {
            await _context.LinkUsersCompany.AddRangeAsync(links);
            await _context.SaveChangesAsync();
        }
    }
}
