using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vertical360_backend.Application.DTOs.Auth;
using Vertical360_backend.Application.Interfaces;
using Vertical360_backend.Domain.Entities;
using Vertical360_backend.Infrastructure.Persistence;

namespace Vertical360_backend.Infrastructure.Auth
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MasterDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;

        public PasswordResetService(
            UserManager<ApplicationUser> userManager,
            MasterDbContext context,
            IEmailService emailService,
            IEmailTemplateService templateService)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
            _templateService = templateService;
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequestDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return;

            // Invalidar OTPs anteriores del usuario
            var existing = await _context.PasswordResetOtps
                .Where(o => o.UserId == user.Id && !o.IsUsed)
                .ToListAsync();
            existing.ForEach(o => o.IsUsed = true);

            var code = Random.Shared.Next(100000, 999999).ToString();

            await _context.PasswordResetOtps.AddAsync(new PasswordResetOtp
            {
                UserId = user.Id,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false
            });
            await _context.SaveChangesAsync();

            var body = await _templateService.RenderAsync("PasswordResetOtp", new Dictionary<string, string>
            {
                { "OTP_CODE", code }
            });

            await _emailService.SendAsync(
                to: model.Email,
                subject: "Recuperación de contraseña - Vertical360",
                body: body);
        }

        public async Task ResetPasswordAsync(ResetPasswordRequestDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email)
                ?? throw new UnauthorizedAccessException("Usuario no encontrado.");

            var otp = await _context.PasswordResetOtps
                .Where(o =>
                    o.UserId == user.Id &&
                    o.Code == model.Otp &&
                    !o.IsUsed &&
                    o.ExpiresAt > DateTime.UtcNow)
                .FirstOrDefaultAsync()
                ?? throw new UnauthorizedAccessException("Código inválido o expirado.");

            otp.IsUsed = true;

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

            if (!result.Succeeded)
                throw new Exception($"Error al cambiar la contraseña: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            await _context.SaveChangesAsync();
        }
    }
}

