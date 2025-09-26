using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vertical360_back.Application.UseCases;
using Vertical360_back.Data;
using Vertical360_back.Domain.Entityes;
using Vertical360_back.Models;

namespace Vertical360_back.Controllers
{
    [Authorize]
    public class ConnectionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceTenant _serviceTenant;
        private IServiceUser _serviceUser;

        public ConnectionsController(ApplicationDbContext context, IServiceTenant serviceTenant, IServiceUser serviceUser)
        {
            _context = context;
            _serviceTenant = serviceTenant;
            _serviceUser = serviceUser;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _serviceUser.GetUserId();
            return await ReturnConnectionsPendings(userId);
        }

        private async Task<IActionResult> ReturnConnectionsPendings(string userId)
        {
            var connectionsPendings = await _context.LinkUsersCommpany
                .Include(x => x.Companies)
                .Where(x => x.statusLink == StatusLinkEnum.Pending
                && x.UserId == userId).ToListAsync();

            return View(connectionsPendings);
        }

        [HttpPost]
        public async Task<IActionResult> Index(Guid companyId, StatusLinkEnum linkStatus)
        {
            var userId = _serviceUser.GetUserId();
            var linked = await _context.LinkUsersCommpany
                    .FirstOrDefaultAsync(x => x.UserId == userId 
                    && x.CompanyId == companyId
                    && x.statusLink == StatusLinkEnum.Pending);

            if (linked is null)
            {
                ModelState.AddModelError("", "Ha habido un error: vinculación no encontrada");
                return await ReturnConnectionsPendings(userId);
            }

            if (linkStatus == StatusLinkEnum.Acepted)
            {
                var permisoNulo = new CompanyUserPermissions()
                {
                    Permissions = Permissions.Null,
                    CompanyId = companyId,
                    UserId = userId
                };

                _context.Add(permisoNulo);
            }

            linked.statusLink = linkStatus;
            await _context.SaveChangesAsync();

            return RedirectToAction("Cambiar", "Empresas");
        }

        public async Task<IActionResult> Linked()
        {
            var companyId = _serviceTenant.GetTenant();

            if (string.IsNullOrEmpty(companyId))
            {
                return RedirectToAction("Index", "Home");
            }

            var companyIdGuid = new Guid(companyId);

            var company = await _context
                .Companies.FirstOrDefaultAsync(x => x.Id == companyIdGuid);

            if (company is null)
            {
                return RedirectToAction("Index", "Home");
            }

            var modelo = new LinkUser
            {
                CompanyId = company.Id,
                NameCompany = company.Name
            };

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Vincular(LinkUser model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuarioAVincular = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == model.EmailUser);

            if (usuarioAVincular is null)
            {
                ModelState.AddModelError(nameof(model.EmailUser), "No existe un usuario con ese email");
                return View(model);
            }

            var linking = new Link
            {
                CompanyId = model.CompanyId,
                UserId = usuarioAVincular.Id,
                Status = StatusLinkEnum.Pending,
                DateCreate = DateTime.UtcNow
            };

            _context.Add(linking);
            await _context.SaveChangesAsync();
            return RedirectToAction("UserLinked");
        }

        public IActionResult UserLinked()
        {
            return View();
        }
    }
}
