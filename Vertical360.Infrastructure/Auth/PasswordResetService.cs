using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vertical360.Application.DTOs.Auth;
using Vertical360.Application.Interfaces;
using Vertical360.Core.Entities;
using Vertical360.Infrastructure.Persistence;

namespace Vertical360.Infrastructure.Auth
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MasterDbContext _context;
        private readonly IEmailService _emailService;

        public PasswordResetService(
            UserManager<ApplicationUser> userManager,
            MasterDbContext context,
            IEmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
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

            await _emailService.SendAsync(
                to: model.Email,
                subject: "Recuperación de contraseña - Vertical360",
                body: $@"
                <h2>Recuperación de contraseña</h2>
                <p>Tu código de verificación es:</p>
                <h1 style='letter-spacing: 8px;'>{code}</h1>
                <p>Este código expira en <strong>10 minutos</strong>.</p>
                <p>Si no solicitaste este código, ignora este mensaje.</p>"
            );
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
