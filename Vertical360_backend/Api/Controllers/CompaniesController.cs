using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Vertical360_backend.Application.DTOs.Companies;
using Vertical360_backend.Application.Interfaces;

namespace Vertical360_backend.Api.Controllers
{
    /// <summary>
    /// Gestión de unidades residenciales (clientes / tenants).
    /// Cada unidad tiene su propia base de datos aislada.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    [Tags("Companies")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
            => _companyService = companyService;

        /// <summary>
        /// Crea una nueva unidad residencial.
        /// Provisiona automáticamente la base de datos del tenant y crea el usuario administrador inicial.
        /// Si no se provee contraseña, se genera una temporal y se envía por email.
        /// </summary>
        [HttpPost]
        [SwaggerOperation(
            Summary     = "Crear unidad residencial",
            Description = "Crea la unidad (compañía/tenant), su base de datos aislada y el usuario administrador inicial. " +
                          "Envía un email de bienvenida con las credenciales al administrador. " +
                          "Si algún paso falla, se ejecuta rollback completo (BD + usuario + registro).",
            OperationId = "Companies_Create")]
        [SwaggerResponse(201, "Unidad creada exitosamente.", typeof(CompanieResultDto))]
        [SwaggerResponse(400, "Datos de entrada inválidos o error en la creación.")]
        [SwaggerResponse(401, "No autenticado.")]
        [SwaggerResponse(409, "Ya existe una unidad con el NIT indicado.")]
        public async Task<IActionResult> Create([FromBody] CompanyRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _companyService.CreateCompanyAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retorna todas las unidades residenciales activas con su información geográfica resuelta.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(
            Summary     = "Listar unidades activas",
            Description = "Devuelve todas las unidades con `IsActive = true`, ordenadas por nombre. " +
                          "Incluye nombres de país, departamento y ciudad resueltos.",
            OperationId = "Companies_GetAll")]
        [SwaggerResponse(200, "Lista de unidades.", typeof(List<CompanieResultDto>))]
        [SwaggerResponse(401, "No autenticado.")]
        [SwaggerResponse(500, "Error interno del servidor.")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _companyService.GetAllAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retorna el detalle de una unidad por su identificador único.
        /// </summary>
        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary     = "Obtener unidad por ID",
            Description = "Busca una unidad activa por su GUID. Incluye todos los campos del negocio.",
            OperationId = "Companies_GetById")]
        [SwaggerResponse(200, "Unidad encontrada.", typeof(CompanieResultDto))]
        [SwaggerResponse(401, "No autenticado.")]
        [SwaggerResponse(404, "Unidad no encontrada.")]
        public async Task<IActionResult> GetById(
            [SwaggerParameter("Identificador único (GUID) de la unidad", Required = true)] Guid id)
        {
            try
            {
                return Ok(await _companyService.GetByIdAsync(id));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza los datos de una unidad existente.
        /// El NIT y el TenantKey no son modificables.
        /// </summary>
        [HttpPut("{id:guid}")]
        [SwaggerOperation(
            Summary     = "Actualizar unidad",
            Description = "Permite actualizar razón social, contacto, ubicación, representante legal y administrador. " +
                          "Los campos no enviados (null) no se modifican. El NIT y el TenantKey son inmutables.",
            OperationId = "Companies_Update")]
        [SwaggerResponse(200, "Unidad actualizada.", typeof(CompanieResultDto))]
        [SwaggerResponse(400, "Datos inválidos.")]
        [SwaggerResponse(401, "No autenticado.")]
        [SwaggerResponse(404, "Unidad no encontrada.")]
        public async Task<IActionResult> Update(
            [SwaggerParameter("ID de la unidad a actualizar", Required = true)] Guid id,
            [FromBody] CompanyUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                return Ok(await _companyService.UpdateAsync(id, dto));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina lógicamente una unidad (IsActive = false). No elimina la base de datos del tenant.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [SwaggerOperation(
            Summary     = "Eliminar unidad (lógico)",
            Description = "Marca la unidad como inactiva (`IsActive = false`). " +
                          "La base de datos del tenant y sus datos se conservan. " +
                          "La unidad no aparecerá en listados pero sus datos son recuperables.",
            OperationId = "Companies_Delete")]
        [SwaggerResponse(204, "Unidad desactivada correctamente.")]
        [SwaggerResponse(401, "No autenticado.")]
        [SwaggerResponse(404, "Unidad no encontrada.")]
        [SwaggerResponse(500, "Error interno.")]
        public async Task<IActionResult> Delete(
            [SwaggerParameter("ID de la unidad a desactivar", Required = true)] Guid id)
        {
            try
            {
                await _companyService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}


