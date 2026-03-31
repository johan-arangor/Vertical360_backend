using Vertical360_backend.Application.Interfaces;

namespace Vertical360_backend.Infrastructure.Auth
{
    /// <summary>
    /// Carga templates desde Infrastructure/EmailTemplates/{templateName}.html
    /// y sustituye placeholders con la sintaxis {{KEY}}.
    /// Los archivos deben estar marcados como CopyToOutputDirectory en el .csproj.
    /// </summary>
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly ILogger<EmailTemplateService> _logger;
        private readonly string _templatesPath;

        public EmailTemplateService(ILogger<EmailTemplateService> logger)
        {
            _logger = logger;
            _templatesPath = Path.Combine(AppContext.BaseDirectory, "EmailTemplates");
        }

        public async Task<string> RenderAsync(string templateName, Dictionary<string, string> placeholders)
        {
            var filePath = Path.Combine(_templatesPath, $"{templateName}.html");

            if (!File.Exists(filePath))
            {
                _logger.LogError("Template de email no encontrado: {FilePath}", filePath);
                throw new FileNotFoundException(
                    $"Template de email '{templateName}' no encontrado en {_templatesPath}.");
            }

            var html = await File.ReadAllTextAsync(filePath);

            // Año actual automático
            placeholders.TryAdd("YEAR", DateTime.UtcNow.Year.ToString());

            foreach (var (key, value) in placeholders)
                html = html.Replace($"{{{{{key}}}}}", value);

            return html;
        }
    }
}
