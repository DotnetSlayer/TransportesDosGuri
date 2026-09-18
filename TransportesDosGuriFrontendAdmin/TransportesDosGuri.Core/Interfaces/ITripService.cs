using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Core.Interfaces
{
    public interface ITripService
    {
        Task<List<TripDTO>> GetAllAsync();
        Task<TripDTO?> GetByIdAsync(long id);
        Task<bool> CreateAsync(TripDTO dto);
        Task<bool> UpdateAsync(long id, TripDTO dto);
        Task<(bool Success, string? Error)> DeleteAsync(long id);
    }
}