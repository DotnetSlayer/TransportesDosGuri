using TransportesDosGuri.Core.Application.DTOs;

namespace TransportesDosGuri.Core.Application.ServiceContracts
{
    public interface ITripService
    {
        Task<IEnumerable<TripDTO>> GetAllAsync();

        Task<TripDTO?> GetByIdAsync(long id);

        Task<TripDTO> CreateAsync(TripDTO trip);

        Task<bool> UpdateAsync(long id, TripDTO trip);

        Task<bool> DeleteAsync(long id);

        Task<IEnumerable<TripDTO>> SearchAsync(string? searchTerm);

        Task<TripDetailsDTO?> GetDetailsAsync(long id);
    }
}
