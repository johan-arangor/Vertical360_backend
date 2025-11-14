using Microsoft.EntityFrameworkCore;
using Vertical360_backend.Application.DTOs.Companies;
using Vertical360_backend.Application.Interfaces;
using Vertical360_backend.Domain.Entities;
using Vertical360_backend.Infrastructure.Persistence;

namespace Vertical360_backend.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;
        public CompanyRepository(ApplicationDbContext context) => _context = context;

        public async Task CreateAsync(Companies company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CompanieResultDto>> GetAllAsync()
        {
            await _context.Companies.AsNoTracking().ToListAsync();

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
            await _context.Companies.FindAsync(id);

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
