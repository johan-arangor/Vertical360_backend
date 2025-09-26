using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Vertical360_back.Application.UseCases;
using Vertical360_back.Infrastructure.Persistence;

namespace Vertical360_back.Security
{
    public class HasPermissionsHandler : AuthorizationHandler<HasPermissionRequirement>
    {
        private readonly IServiceTenant _serviceTenant;
        private readonly IServiceUser _serviceUser;
        private readonly ApplicationDbContext _context;

        public HasPermissionsHandler(IServiceTenant serviceTenant, IServiceUser serviceUser, ApplicationDbContext context)
        {
            _serviceTenant = serviceTenant;
            _serviceUser = serviceUser;
            _context = context;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionRequirement requirement)
        {
            var permission = requirement.Permission;
            var userId = _serviceUser.GetUserId();
            var tenantId = new Guid(_serviceTenant.GetTenant());

            var hasPrmission = await _context.CompanyUserPermissions
                .AnyAsync(x => x.UserId == userId
                && x.CompanyId == tenantId && x.Permissions == permission);

            if (hasPrmission)
            {
                context.Succeed(requirement);
            }
        }
    }
}
