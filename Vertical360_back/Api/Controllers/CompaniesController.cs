using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vertical360_back.Application.Common.Errors;
using Vertical360_back.Application.Common.Exceptions;
using Vertical360_back.Application.Common.Responses;
using Vertical360_back.Application.Contracts.Auth;
using Vertical360_back.Application.Contracts.Company;
using Vertical360_back.Services.Company;

namespace Vertical360_back.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompaniesController : BaseController
    {
        private readonly ICompanyService _service;

        public CompaniesController(ICompanyService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateCompanyDto model)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    throw new AppException("USER_UNAUTHORIZED", "No se pudo identificar al usuario autenticado.");
                }

                var userId = Guid.Parse(userIdClaim);
                var company = await _service.CreateCompanyAsync(model, userId);

                return Ok(new { success = true, company });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<CompanyDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var companies = await _service.GetAllCompaniesAsync();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var company = await _service.GetCompanyByIdAsync(id);
                if (company == null)
                {
                    throw new AppException("COMPANY_NOT_FOUND", $"No se encontró la compañía con ID {id}");
                }

                return Ok(company);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(Guid id, [FromBody] string name)
        {
            try
            {
                await _service.UpdateCompanyAsync(id, name);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteCompanyAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
