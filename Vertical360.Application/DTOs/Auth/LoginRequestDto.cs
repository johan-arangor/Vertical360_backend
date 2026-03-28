namespace Vertical360.Application.DTOs.Auth
{
    /// <summary>
    /// Objeto de transferencia de datos para el inicio de sesión.
    /// Soporta tanto UserName como Email indistintamente para compatibilidad con el frontend.
    /// </summary>
    public class LoginRequestDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool RememberMe { get; set; } = false;
        
        /// <summary>
        /// Opcional: Si se proporciona, el generador de tokens incluirá el tenantId de esta compañía.
        /// </summary>
        public Guid? CompanyId { get; set; }
    }
}
