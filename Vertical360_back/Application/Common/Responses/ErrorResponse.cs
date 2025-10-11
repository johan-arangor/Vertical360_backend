namespace Vertical360_back.Application.Common.Responses
{
    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; } = null;
    }
}
