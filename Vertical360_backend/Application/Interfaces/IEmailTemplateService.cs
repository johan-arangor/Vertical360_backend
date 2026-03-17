namespace Vertical360_backend.Application.Interfaces
{
    /// <summary>
    /// Carga templates HTML de email desde el sistema de archivos
    /// y reemplaza los placeholders {{KEY}} con los valores provistos.
    /// </summary>
    public interface IEmailTemplateService
    {
        /// <summary>
        /// Carga el template por nombre (sin extensión) y reemplaza
        /// los placeholders con el diccionario de valores.
        /// </summary>
        /// <param name="templateName">Nombre del archivo sin .html (ej: "PasswordResetOtp")</param>
        /// <param name="placeholders">Diccionario clave→valor para reemplazar en el template</param>
        Task<string> RenderAsync(string templateName, Dictionary<string, string> placeholders);
    }
}
