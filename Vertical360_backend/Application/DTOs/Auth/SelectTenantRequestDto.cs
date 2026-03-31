namespace Vertical360_backend.Application.DTOs.Auth
{
    /// <summary>Payload para el paso 2 del login: selección de tenant.</summary>
    public class SelectTenantRequestDto
    {
        /// <summary>Token pre_auth obtenido en el paso 1 (/login). Válido por 2 minutos.</summary>
        public string PreAuthToken { get; set; } = string.Empty;

        /// <summary>GUID de la unidad residencial (compañía) a la que el usuario desea acceder.</summary>
        public string CompanyId { get; set; } = string.Empty;
    }
}

