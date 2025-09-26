using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Vertical360_back.Data;
using Vertical360_back.Models;
using Vertical360_back.Security;
using Vertical360_back.Domain.Entityes;
using Vertical360_back.Application.UseCases;
using Vertical360_back.Common;

namespace Vertical360_back.Controllers
{
    [Authorize]
    public class PermissionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceTenant _serviceTenant;

        public PermissionsController(ApplicationDbContext context, IServiceTenant serviceTenant)
        {
            _context = context;
            _serviceTenant = serviceTenant;
        }

        [HasPermission(Permissions.Permission_Read)]
        public async Task<ActionResult> Index()
        {
            var tenantId = new Guid(_serviceTenant.GetTenant());
            var model = await _context.Companies.
                Include(x => x.CompanyUserPermissions).ThenInclude(x => x.User)
                .Where(x => x.Id == tenantId)
                .Select(x => new IndexPermisosDTO
                {
                    CompanyName = x.Name,
                    Employeds = x.CompanyUserPermissions.Select(y => new UserDTO
                    {
                        Email = y.User!.Email!
                    }).Distinct()
                }).FirstOrDefaultAsync();

            return View(model);

        }

        [HasPermission(Permissions.Permission_Read)]
        public async Task<IActionResult> Administration(string email)
        {
            var tenantId = new Guid(_serviceTenant.GetTenant());
            var userId = await _context.Users
                .Where(x => x.Email == email).Select(x => x.Id).FirstOrDefaultAsync();

            if (userId is null)
            {
                return RedirectToAction("Index", "Permissions");
            }

            var permissions = await _context.CompanyUserPermissions
                                .Where(x => x.CompanyId == tenantId && x.UserId == userId
                                    && x.Permissions != Permissions.Null).ToListAsync();

            var permissionsUserDictionary = permissions.ToDictionary(x => x.Permissions);

            var modelo = new AdminPermissionsDTO();
            modelo.UserId = userId;
            modelo.Email = email;

            foreach (var permission in Enum.GetValues<Permissions>())
            {
                var field = typeof(Permissions).GetField(permission.ToString())!;
                var hide = field.IsDefined(typeof(HideAttribute), false);

                if (hide)
                {
                    continue;
                }

                var description = permission.ToString();

                if (field.IsDefined(typeof(DisplayAttribute), false))
                {
                    var displayAttr = (DisplayAttribute)Attribute
                        .GetCustomAttribute(field, typeof(DisplayAttribute))!;
                    description = displayAttr.Description;
                }

                modelo.Permissions.Add(new PermissionUserDTO()
                {
                    Description = description,
                    Permission = permission,
                    Assigned = permissionsUserDictionary.ContainsKey(permission)
                });

            }

            return View(modelo);

        }

        [HasPermission(Permissions.Permission_Update)]
        [HttpPost]
        public async Task<IActionResult> Administration(AdminPermissionsDTO modelo)
        {
            var tenantId = new Guid(_serviceTenant.GetTenant());

            // Siempre agregamos el permiso por defecto.
            modelo.Permissions.Add(new PermissionUserDTO()
            { Assigned = true, Permission = Permissions.Null });

            // Borramos todos los permisos de la persona
            await _context.Database
                .ExecuteSqlInterpolatedAsync($@"DELETE CompanyUserPermissions 
                   WHERE UserId = {modelo.UserId} AND CompanyId = {tenantId}");

            // Filtramos los permisos a agregar
            var permissionsFiltered = modelo.Permissions
                .Where(x => x.Assigned).Select(x => new CompanyUserPermissions
                {
                    CompanyId = tenantId,
                    UserId = modelo.UserId,
                    Permissions = x.Permission
                });

            // Agregamos los permisos a la tabla
            _context.AddRange(permissionsFiltered);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
