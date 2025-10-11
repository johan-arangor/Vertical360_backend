using Microsoft.AspNetCore.Mvc;
using Vertical360_back.Application.Common.Errors;
using Vertical360_back.Application.Common.Exceptions;
using Vertical360_back.Application.Common.Responses;

namespace Vertical360_back.Api.Controllers
{
    [ApiController]
    public abstract class BaseController : Controller
    {
        protected IActionResult HandleException(Exception ex)
        {
            if (ex is AppException appEx)
            {
                var message = ErrorCatalog.Messages.ContainsKey(appEx.Code)
                    ? ErrorCatalog.Messages[appEx.Code]
                    : ErrorCatalog.Messages["INTERNAL_SERVER_ERROR"];

                return appEx.Code switch
                {
                    string code when code.StartsWith("USER_") =>
                        BadRequest(new ErrorResponse
                        {
                            Code = appEx.Code,
                            Message = message,
                            Details = appEx.Message
                        }),

                    string code when code.StartsWith("COMPANY_") =>
                        BadRequest(new ErrorResponse
                        {
                            Code = appEx.Code,
                            Message = message,
                            Details = appEx.Message
                        }),

                    _ => StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                    {
                        Code = "INTERNAL_SERVER_ERROR",
                        Message = ErrorCatalog.Messages["INTERNAL_SERVER_ERROR"],
                        Details = ex.Message
                    })
                };
            }

            // Excepciones comunes no controladas
            if (ex is UnauthorizedAccessException)
            {
                return Unauthorized(new ErrorResponse
                {
                    Code = "USER_UNAUTHORIZED",
                    Message = ErrorCatalog.Messages["USER_UNAUTHORIZED"],
                    Details = ex.Message
                });
            }

            if (ex is KeyNotFoundException)
            {
                return NotFound(new ErrorResponse
                {
                    Code = "NOT_FOUND",
                    Message = ErrorCatalog.Messages["COMPANY_NOT_FOUND"],
                    Details = ex.Message
                });
            }

            // Fallback para cualquier otro error no controlado
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
            {
                Code = "INTERNAL_SERVER_ERROR",
                Message = ErrorCatalog.Messages["INTERNAL_SERVER_ERROR"],
                Details = ex.Message
            });
        }
    }
}
