using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vertical360_back.Application.Common.Exceptions;
using Vertical360_back.Application.Contracts.Resident;
using Vertical360_back.Domain.Entityes.Tenants;
using Vertical360_back.Infrastructure.Persistence.Repositories.Tenants;

namespace Vertical360_back.Api.Controllers.Tenants
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ResidentController : BaseController
    {
        private readonly IResidentRepository _residentRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ResidentController> _logger;

        public ResidentController(IResidentRepository residentRepository, UserManager<IdentityUser> userManager, ILogger<ResidentController> logger)
        {
            _residentRepository = residentRepository;
            _userManager = userManager;
            _logger = logger;
        }

        private string GetTenantId()
        {
            var tenantId = User.FindFirstValue("tenantId");
            if (string.IsNullOrEmpty(tenantId))
                throw new UnauthorizedAccessException("No se encontró el TenantId en los claims del usuario.");
            return tenantId;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Resident), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Create([FromBody] ResidentRequestDto model)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    throw new AppException("USER_UNAUTHORIZED", "No se pudo identificar al usuario autenticado.");
                }

                var userId = Guid.Parse(userIdClaim);

                var tenantId = GetTenantId();

                if (await _residentRepository.ExistsByDocumentAsync(model.Document, tenantId))
                    return BadRequest(new { message = "Ya existe un residente con este documento." });

                var resident = await CreateResidentAction(tenantId, userId, model);

                if (model.IsUser && resident != null)
                {
                    Console.WriteLine("Aca ira a crear el residente como un usuario para que se pueda logear, pero con rol de resident o owner");
                }

                return CreatedAtAction(nameof(GetById), new { id = resident.Id }, resident);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear residente.");
                return HandleException(ex);
            }
        }
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Resident>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var tenantId = GetTenantId();
                var residents = await _residentRepository.GetAllAsync(tenantId);
                return Ok(residents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener residentes.");
                return HandleException(ex);
            }
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(Resident), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var resident = await _residentRepository.GetByIdAsync(id, tenantId);
                if (resident == null)
                    return NotFound(new { message = $"No se encontró el residente con Id {id}" });

                return Ok(resident);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener residente por Id.");
                return HandleException(ex);
            }
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ResidentRequestDto model)
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    throw new AppException("USER_UNAUTHORIZED", "No se pudo identificar al usuario autenticado.");
                }

                var userId = Guid.Parse(userIdClaim);
                var tenantId = GetTenantId();
                var existing = await _residentRepository.GetByIdAsync(id, tenantId);

                if (existing == null)
                    return NotFound(new { message = $"No se encontró el residente con Id {id}" });

                await UpdateResidentAction(tenantId, userId, existing, model);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar residente.");
                return HandleException(ex);
            }
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                await _residentRepository.DeleteAsync(id, tenantId);
                await _residentRepository.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar residente.");
                return HandleException(ex);
            }
        }

        private async Task<ResidentResponseDto> CreateResidentAction(string tenantId, Guid userId, ResidentRequestDto model)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new AppException("USER_NOT_FOUND", "El usuario autenticado no existe.");

            var resident = new Resident
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                FirstName = model.FirtsName,
                MiddleName = model.MiddleName ?? string.Empty,
                LastName = model.LastName,
                MiddleLastName = model.MiddleLastName ?? string.Empty,
                Document = model.Document,
                DocumentType = model.DocumentType,
                IsActive = model.IsActive,
                IsOwner = model.IsOwner,
                CreatedAt = DateTime.UtcNow,
                UserIdentity = user
            };

            if (model.AdditionalInfo != null)
            {
                resident.AdditionalInfo = new ResidentAdditionalInfo
                {
                    TenantId = tenantId,
                    DateOfBirth = model.AdditionalInfo.DateOfBirth,
                    GenderType = model.AdditionalInfo.GenderType
                };
            }

            await _residentRepository.AddAsync(resident);
            await _residentRepository.SaveChangesAsync();

            return new ResidentResponseDto
            {
                Id = resident.Id,
                FullName = resident.FullName,
                Document = resident.Document,
                DocumentType = resident.DocumentType,
                IsOwner = resident.IsOwner,
                IsActive = resident.IsActive,
                DateOfBirth = resident.AdditionalInfo?.DateOfBirth,
                GenderType = resident.AdditionalInfo?.GenderType
            };
        }

        private async Task UpdateResidentAction(string tenantId, Guid userId, Resident resident, ResidentRequestDto model)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new AppException("USER_NOT_FOUND", "El usuario autenticado no existe.");

            resident.FirstName = model.FirtsName;
            resident.MiddleName = model.MiddleName;
            resident.LastName = model.LastName;
            resident.MiddleLastName = model.MiddleLastName;
            resident.Document = model.Document;
            resident.DocumentType = model.DocumentType;
            resident.IsActive = model.IsActive;
            resident.IsOwner = model.IsOwner;
            resident.UserIdentity = user;

            if (model.AdditionalInfo != null)
            {
                if (resident.AdditionalInfo == null)
                {
                    resident.AdditionalInfo.TenantId = tenantId;
                }

                resident.AdditionalInfo.DateOfBirth = model.AdditionalInfo.DateOfBirth;
                resident.AdditionalInfo.GenderType = model.AdditionalInfo.GenderType;
            }

            await _residentRepository.UpdateAsync(resident);
            await _residentRepository.SaveChangesAsync();
        }
    }
}
