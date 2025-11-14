using Vertical360_backend.Application.DTOs.Residents;
using Vertical360_backend.Application.Interfaces;
using Vertical360_backend.Domain.Entities;

namespace Vertical360_backend.Infrastructure.Services
{
    public class ResidentService : IResidentService
    {
        private readonly IResidentRepository _repository;
        private readonly ICurrentTenantService _tenantService;

        public ResidentService(IResidentRepository repository, ICurrentTenantService tenantService)
        {
            _repository = repository;
            _tenantService = tenantService;
        }

        public async Task<List<ResidentResultDto>> GetAllAsync()
        {
            var tenantId = _tenantService.TenantId;
            var residents = await _repository.GetAllAsync(tenantId);

            return residents.ToList();
        }

        public async Task<ResidentResultDto> GetByIdAsync(Guid id)
        {
            var tenantId = _tenantService.TenantId;
            var resident = await _repository.GetByIdAsync(id, tenantId)
                ?? throw new Exception("Residente no encontrado");
            
            return MapToResponse(resident);
        }

        public async Task<ResidentResultDto> CreateAsync(ResidentRequestDto dto)
        {
            var tenantId = _tenantService.TenantId;
            var resident = new Resident
            {
                FirstName = dto.FirstName,
                SecondName = dto.SecondName,
                LastName = dto.LastName,
                SecondLastName = dto.SecondLastName,
                Document = dto.Document,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                TenantId = tenantId,
                IsOwner = dto.IsOwner
            };

            await _repository.AddAsync(resident);

            return MapToResponse(resident);
        }

        public async Task<ResidentResultDto> UpdateAsync(Guid id, ResidentRequestDto dto)
        {
            var tenantId = _tenantService.TenantId;
            var existing = await _repository.GetByIdAsync(id, tenantId)
                ?? throw new Exception("Residente no encontrado");

            existing.FirstName = dto.FirstName;
            existing.SecondName = dto.SecondName;
            existing.LastName = dto.LastName;
            existing.SecondLastName = dto.SecondLastName;
            existing.Document = dto.Document;
            existing.Email = dto.Email;
            existing.PhoneNumber = dto.PhoneNumber;
            existing.IsOwner = dto.IsOwner;

            await _repository.UpdateAsync(existing);

            return MapToResponse(existing);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            return true;
        }

        private static ResidentResultDto MapToResponse(Resident r) => new()
        {
            Id = r.Id,
            FullName = r.FullName,
            Document = r.Document,
            Email = r.Email,
            PhoneNumber = r.PhoneNumber,
            IsActive = r.IsActive,
            IsOwner = r.IsOwner
        };
    }
}
