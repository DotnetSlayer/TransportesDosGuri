using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface IAirportService
    {
        Task<List<AirportDTO>> GetAllAsync();
        Task<AirportDTO?> GetByIdAsync(long id);
        Task<bool> CreateAsync(AirportDTO dto);
        Task<bool> UpdateAsync(long id, AirportDTO dto);
        Task<(bool Success, string? Error)> DeleteAsync(long id);
    }
}