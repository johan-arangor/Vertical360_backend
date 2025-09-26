using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Vertical360_back.Application.UseCases;
using Vertical360_back.Domain.Entityes;
using Vertical360_back.Infrastructure.Persistence;
using Vertical360_back.Models;

namespace Vertical360_back.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IServiceChangeTenant _serviceChangeTenant;
        private readonly ApplicationDbContext _context;

        public UsersController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IServiceChangeTenant serviceChangeTenant, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _serviceChangeTenant = serviceChangeTenant;
            _context = context;
        }

        public IActionResult Registry()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registry(RegistryViewModel model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }

            var user = new IdentityUser { UserName = model.Email, Email = model.Email };

            var result = await _userManager.CreateAsync(user, password: model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var companiesLink = await _context.CompanyUserPermissions
                                        .Where(x => x.UserId== user!.Id && x.Permissions == Permissions.Null)
                                        .OrderBy(x => x.CompanyId)
                                        .Take(2)
                                        .Select(x => x.CompanyId).ToListAsync();

                if (companiesLink.Count == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (companiesLink.Count == 1)
                {
                    await _serviceChangeTenant.ReplaceTenant(companiesLink[0], user!.Id);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Change", "Companies");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o contraseña incorrectas");
                return View(model);
            }
        }
    }
}
