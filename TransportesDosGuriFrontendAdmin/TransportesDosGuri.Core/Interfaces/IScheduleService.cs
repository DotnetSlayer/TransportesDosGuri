using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface IScheduleService
    {
        Task<List<ScheduleDTO>> GetAllAsync();
        Task<ScheduleDTO?> GetByIdAsync(long id);
        Task<bool> CreateAsync(ScheduleDTO dto);
        Task<bool> UpdateAsync(long id, ScheduleDTO dto);
        Task<(bool Success, string? Error)> DeleteAsync(long id);
    }
}