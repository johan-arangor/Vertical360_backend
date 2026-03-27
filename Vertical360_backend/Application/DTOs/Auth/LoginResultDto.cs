namespace Vertical360_backend.Application.DTOs.Auth
{
    /// <summary>Resultado del proceso de autenticación.</summary>
    public class LoginResultDto
    {
        /// <summary>ID del usuario autenticado.</summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>Rol principal del usuario en el tenant seleccionado.</summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>Email del usuario.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Token JWT. En el paso 1 es un pre_auth token (2 min).
        /// En el paso 2 es el JWT definitivo con tenant_id.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>Indica si la operación fue exitosa.</summary>
        public bool Success { get; set; } = false;

        /// <summary>Lista de unidades residenciales asociadas al usuario (solo en paso 1).</summary>
        public List<ClientInfoDto> AssociatedClients { get; set; } = new();
    }
}

