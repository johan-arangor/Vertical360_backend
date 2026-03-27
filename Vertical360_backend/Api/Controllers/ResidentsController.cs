using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Vertical360_backend.Application.DTOs.Residents;
using Vertical360_backend.Application.Interfaces;

namespace Vertical360_backend.Api.Controllers
{
    /// <summary>
    /// Gestión de residentes de una unidad residencial (tenant).
    /// Todos los endpoints operan sobre la BD del tenant activo en el JWT.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    [Tags("Residents")]
    public class ResidentsController : ControllerBase
    {
        private readonly IResidentService _service;

        public ResidentsController(IResidentService service) => _service = service;

        /// <summary>
        /// Retorna todos los residentes activos del tenant autenticado.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(
            Summary     = "Listar residentes",
            Description = "Devuelve todos los residentes activos de la unidad residencial del JWT actual.",
            OperationId = "Residents_GetAll")]
        [SwaggerResponse(200, "Lista de residentes.", typeof(List<ResidentResultDto>))]
        [SwaggerResponse(401, "No autenticado o tenant no resuelto.")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retorna un residente por su identificador único.
        /// </summary>
        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary     = "Obtener residente por ID",
            Description = "Busca el residente por GUID dentro del tenant activo.",
            OperationId = "Residents_GetById")]
        [SwaggerResponse(200, "Residente encontrado.", typeof(ResidentResultDto))]
        [SwaggerResponse(401, "No autenticado.")]
        [SwaggerResponse(404, "Residente no encontrado.")]
        public async Task<IActionResult> GetById(
            [SwaggerParameter("Identificador único del residente", Required = true)] Guid id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo residente en la unidad residencial activa.
        /// </summary>
        [HttpPost]
        [SwaggerOperation(
            Summary     = "Crear residente",
            Description = "Registra un nuevo residente en la BD del tenant activo. " +
                          "El campo `Document` es único por unidad residencial.",
            OperationId = "Residents_Create")]
        [SwaggerResponse(201, "Residente creado.", typeof(ResidentResultDto))]
        [SwaggerResponse(400, "Datos inválidos o documento duplicado.")]
        [SwaggerResponse(401, "No autenticado.")]
        public async Task<IActionResult> Create([FromBody] ResidentRequestDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza los datos de un residente existente.
        /// </summary>
        [HttpPut("{id:guid}")]
        [SwaggerOperation(
            Summary     = "Actualizar residente",
            Description = "Modifica los datos del residente identificado por GUID dentro del tenant activo.",
            OperationId = "Residents_Update")]
        [SwaggerResponse(200, "Residente actualizado.", typeof(ResidentResultDto))]
        [SwaggerResponse(400, "Datos inválidos.")]
        [SwaggerResponse(401, "No autenticado.")]
        [SwaggerResponse(404, "Residente no encontrado.")]
        public async Task<IActionResult> Update(
            [SwaggerParameter("ID del residente a actualizar", Required = true)] Guid id,
            [FromBody] ResidentRequestDto dto)
        {
            try
            {
                var result = await _service.UpdateAsync(id, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina lógicamente un residente (IsActive = false).
        /// </summary>
        [HttpDelete("{id:guid}")]
        [SwaggerOperation(
            Summary     = "Eliminar residente (lógico)",
            Description = "Marca el residente como inactivo. Sus datos se conservan en la BD.",
            OperationId = "Residents_Delete")]
        [SwaggerResponse(204, "Residente desactivado.")]
        [SwaggerResponse(401, "No autenticado.")]
        [SwaggerResponse(404, "Residente no encontrado.")]
        public async Task<IActionResult> Delete(
            [SwaggerParameter("ID del residente a desactivar", Required = true)] Guid id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}

