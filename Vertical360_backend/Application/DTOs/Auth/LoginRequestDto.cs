namespace Vertical360_backend.Application.DTOs.Auth
{
    /// <summary>Credenciales para el inicio de sesión (paso 1).</summary>
    public class LoginRequestDto
    {
        /// <summary>Nombre de usuario (cédula o email).</summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>Email del usuario (alternativo a UserName).</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Contraseña del usuario.</summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>Mantener la sesión activa.</summary>
        public bool RememberMe { get; set; } = false;

        /// <summary>ID de compañía opcional. Reservado para uso futuro.</summary>
        public Guid? CompanyId { get; set; }
    }
}

