using Vertical360.Application.DTOs.Residents;

namespace Vertical360.Application.Interfaces
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
