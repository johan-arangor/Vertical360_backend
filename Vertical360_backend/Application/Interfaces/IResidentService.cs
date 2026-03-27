using Vertical360_backend.Application.DTOs.Residents;

namespace Vertical360_backend.Application.Interfaces
{
    public interface IResidentService
    {
        Task<List<ResidentResultDto>> GetAllAsync();
        Task<ResidentResultDto> GetByIdAsync(Guid id);
        Task<ResidentResultDto> CreateAsync(ResidentRequestDto dto);
        Task<ResidentResultDto> UpdateAsync(Guid id, ResidentRequestDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
