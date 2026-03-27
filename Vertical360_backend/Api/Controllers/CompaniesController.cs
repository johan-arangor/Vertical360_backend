using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vertical360.Application.DTOs.Companies;
using Vertical360.Application.Interfaces;
using Vertical360.Core.Entities;

namespace Vertical360_backend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService) => _companyService = companyService;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CompanyRequestDto company)
        {
            try
            {
                await _companyService.CreateCompanyAsync(company);
                return Ok(new { message = "Company created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _companyService.GetAllAsync());
    }
}
