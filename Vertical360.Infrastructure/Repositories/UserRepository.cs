using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vertical360.Application.Interfaces;
using Vertical360.Core.Entities;
using Vertical360.Infrastructure.Persistence;

namespace Vertical360.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MasterDbContext _context;

        public UserRepository(UserManager<ApplicationUser> userManager, MasterDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
            => await _userManager.FindByIdAsync(id);

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync(string tenantId)
            => await _context.Users
                .Where(u => u.TenantId == tenantId || u.TenantId == null)
                .ToListAsync();

        public async Task CreateAsync(ApplicationUser user, string password, string role, string tenantId)
        {
            user.TenantId = tenantId;
            var res = await _userManager.CreateAsync(user, password);
            if (!res.Succeeded)
                throw new Exception(string.Join("; ", res.Errors.Select(e => e.Description)));
            await _userManager.AddToRoleAsync(user, role);
        }
    }
}
