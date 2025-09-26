using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Vertical360_back.Application.UseCases;
using Vertical360_back.Data;

namespace Vertical360_back.Services
{
    public class ServiceChangeTenant : IServiceChangeTenant
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public ServiceChangeTenant(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task ReplaceTenant(Guid companyId, string userId)
        {
            var usersInCompany = await _userManager.FindByIdAsync(userId);

            var claimTenantExist = await _context.UserClaims
                .FirstOrDefaultAsync(x => x.ClaimType == Constants.CLAIM_TENANT && x.UserId == userId);

            if(claimTenantExist is not null)
            {
                _context.Remove(claimTenantExist);
            }

            var claimNewTenant = new Claim(Constants.CLAIM_TENANT, companyId.ToString());

            await _userManager.AddClaimAsync(usersInCompany!, claimNewTenant);

            await _signInManager.SignInAsync(usersInCompany!, isPersistent: true);
        }
    }
}
