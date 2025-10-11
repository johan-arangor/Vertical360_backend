namespace Vertical360_back.Application.Common.Errors
{
    public static class ErrorCatalog
    {
        public static readonly Dictionary<string, string> Messages = new Dictionary<string, string>()
        { 
            // --- Errores de autenticación / usuario ---
            { "USER_WEAK_PASSWORD", "La contraseña es demasiado débil. Debe incluir una mayúscula, un número y un carácter especial." },
            { "USER_EMAIL_EXISTS", "Ya existe un usuario con este correo electrónico." },
            { "USER_NOT_FOUND", "El usuario no fue encontrado." },
            { "USER_UNAUTHORIZED", "No tiene permisos para realizar esta acción." },
            { "USER_CREATION_FAILED", "No se pudo crear el usuario administrador de la compañía." },

            // --- Errores de compañía ---
            { "COMPANY_NOT_FOUND", "La compañía no existe." },
            { "COMPANY_DUPLICATE", "Ya existe una compañía con este nombre." },
            { "COMPANY_CREATION_FAILED", "No se pudo crear la compañía." },

            // --- Errores genéricos ---
            { "VALIDATION_ERROR", "Error de validación de datos." },
            { "INTERNAL_SERVER_ERROR", "Error interno del servidor. Intente nuevamente más tarde." }

        };
    }
}
