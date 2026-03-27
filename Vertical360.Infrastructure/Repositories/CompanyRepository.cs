using Microsoft.EntityFrameworkCore;
using Vertical360.Application.DTOs.Companies;
using Vertical360.Application.Interfaces;
using Vertical360.Core.Entities;
using Vertical360.Infrastructure.Persistence;

namespace Vertical360.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly MasterDbContext _context;

        public CompanyRepository(MasterDbContext context) => _context = context;

        public async Task CreateAsync(Companies company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CompanieResultDto>> GetAllAsync()
        {
            return await _context.Companies
                .AsNoTracking()
                .Select(c => new CompanieResultDto
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .ToListAsync();
        }

        public async Task<CompanieResultDto?> GetByIdAsync(Guid id)
        {
            return await _context.Companies
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CompanieResultDto
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .FirstOrDefaultAsync();
        }
    }
}
