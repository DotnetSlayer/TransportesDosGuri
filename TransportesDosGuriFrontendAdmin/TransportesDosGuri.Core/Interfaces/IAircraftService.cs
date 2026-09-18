using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface IAircraftService
    {
        Task<List<AircraftDTO>> GetAllAsync();
        Task<AircraftDTO?> GetByIdAsync(long id);
        Task<bool> CreateAsync(AircraftDTO dto);
        Task<bool> UpdateAsync(long id, AircraftDTO dto);
        Task<(bool Success, string? Error)> DeleteAsync(long id);
    }
}