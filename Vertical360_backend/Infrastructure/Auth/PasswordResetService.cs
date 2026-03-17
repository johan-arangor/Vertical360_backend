
using Microsoft.AspNetCore.Identity;
using System.Data.Entity;
using Vertical360_backend.Application.DTOs.Auth;
using Vertical360_backend.Application.Interfaces;
using Vertical360_backend.Domain.Entities;
using Vertical360_backend.Infrastructure.Persistence;

namespace Vertical360_backend.Infrastructure.Auth
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public PasswordResetService(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IEmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequestDto model)
        {
            // Siempre responde igual para no revelar si el email existe
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return;

            // Invalidar OTPs anteriores del usuario
            var existing = await _context.PasswordResetOtps
                .Where(o => o.UserId == user.Id && !o.IsUsed)
                .ToListAsync();
            existing.ForEach(o => o.IsUsed = true);

            // Generar OTP de 6 dígitos
            var code = Random.Shared.Next(100000, 999999).ToString();

            var otp = new PasswordResetOtp
            {
                UserId = user.Id,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false
            };

            await _context.PasswordResetOtps.AddAsync(otp);
            await _context.SaveChangesAsync();

            // Enviar email
            await _emailService.SendAsync(
                to: model.Email,
                subject: "Recuperación de contraseña - Vertical360",
                body: $@"
                <h2>Recuperación de contraseña</h2>
                <p>Tu código de verificación es:</p>
                <h1 style='letter-spacing: 8px;'>{code}</h1>
                <p>Este código expira en <strong>10 minutos</strong>.</p>
                <p>Si no solicitaste este código, ignora este mensaje.</p>
            "
            );
        }

        public async Task ResetPasswordAsync(ResetPasswordRequestDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email)
                ?? throw new UnauthorizedAccessException("Usuario no encontrado.");

            // Buscar OTP válido
            var otp = await _context.PasswordResetOtps
                .Where(o =>
                    o.UserId == user.Id &&
                    o.Code == model.Otp &&
                    !o.IsUsed &&
                    o.ExpiresAt > DateTime.UtcNow)
                .FirstOrDefaultAsync()
                ?? throw new UnauthorizedAccessException("Código inválido o expirado.");

            // Marcar OTP como usado
            otp.IsUsed = true;

            // Resetear contraseña usando Identity
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Error al cambiar la contraseña: {errors}");
            }

            await _context.SaveChangesAsync();
        }
    }
}
