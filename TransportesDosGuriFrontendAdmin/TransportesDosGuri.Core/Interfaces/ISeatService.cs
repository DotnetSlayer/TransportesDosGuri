using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface ISeatService
    {
        Task<List<SeatDTO>> GetAllAsync();
        Task<SeatDTO?> GetByIdAsync(long id);
        Task<bool> CreateAsync(SeatDTO dto);
        Task<bool> UpdateAsync(long id, SeatDTO dto);
        Task<(bool Success, string? Error)> DeleteAsync(long id);
    }
}