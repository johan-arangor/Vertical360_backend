using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using Vertical360_back.Data;
using Vertical360_back.Entityes;
using Vertical360_back.Models;
using Vertical360_back.Services;

namespace Vertical360_back.Controllers
{
    [Authorize]
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceUser _serviceUser;
        private readonly IServiceChangeTenant _serviceChangeTenant;

        public CompaniesController(ApplicationDbContext context, IServiceUser serviceUser, IServiceChangeTenant serviceChangeTenant)
        {
            _context = context;
            _serviceUser = serviceUser;
            _serviceChangeTenant = serviceChangeTenant;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCompany createCompany)
        {
            if (!ModelState.IsValid)
            {
                return View(createCompany);
            }

            var company = new Companies
            {
                Name = createCompany.Name
            };

            var userId = _serviceUser.GetUserId();

            company.UserCreationId = userId;
            _context.Add(company);

            await _context.SaveChangesAsync();

            // Le damos todos los permisos al usuario que crea la app.
            var userCompanyPermission = new List<CompanyUserPermissions>();

            foreach (var permission in Enum.GetValues<Permissions>())
            {
                userCompanyPermission.Add(new CompanyUserPermissions
                {
                    CompanyId = company.Id,
                    UserId = userId!,
                    Permissions = permission
                });
            }

            _context.AddRange(userCompanyPermission);

            await _context.SaveChangesAsync();

            await _serviceChangeTenant.ReplaceTenant(company.Id, userId);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Change()
        {
            var userId = _serviceUser.GetUserId();
            var companies = await _context.CompanyUserPermissions
                .Include(cup => cup.Companies)
                .Where(cup => cup.UserId == userId)
                .Select(cup => cup.Companies!).Distinct().ToListAsync();

            return View(companies);
        }

        [HttpPost]
        public async Task<IActionResult> Change(Guid companyId)
        {
            var userId = _serviceUser.GetUserId();

            await _serviceChangeTenant.ReplaceTenant(companyId, userId);

            return RedirectToAction("Index", "Home");
        }
    }
}
