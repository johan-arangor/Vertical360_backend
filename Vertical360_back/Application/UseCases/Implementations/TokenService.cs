using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Vertical360_back.Application.Configuration;
using Vertical360_back.Application.Interfaces.Services;
using Vertical360_back.Domain.Enums;
using Vertical360_back.Infrastructure.Persistence;

namespace Vertical360_back.Application.UseCases.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TokenService(IOptions<JwtSettings> jwtSettings, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _jwtSettings = jwtSettings.Value;
            _userManager = userManager;
            _context = context;
        }

        public async Task<string> GenerateTokenAsync(IdentityUser user)
        {
            // Buscar el tenantId del usuario (depende de tu modelo de enlace)
            var link = await _context.LinkUsersCompany
                .FirstOrDefaultAsync(l => l.UserId == user.Id && l.statusLink == StatusLinkEnum.Accepted);

            if (link == null)
            {
                throw new Exception("El usuario no está asociado a ninguna compañía (tenant).");
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email),
                new Claim("tenantId", link.CompanyId.ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, _jwtSettings.Issuer),
                new Claim(JwtRegisteredClaimNames.Aud, _jwtSettings.Audience)
            };

            // Agregar roles al token
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Clave de seguridad (obtenida de la configuración)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Creación del token
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
