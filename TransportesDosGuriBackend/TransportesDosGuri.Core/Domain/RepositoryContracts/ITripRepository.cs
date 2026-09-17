using TransportesDosGuri.Core.Application.DTOs;
using TransportesDosGuri.Core.Domain.Entities;

namespace TransportesDosGuri.Core.Domain.RepositoryContracts
{
    public interface ITripRepository
    {
        Task<IEnumerable<Trip>> GetAllAsync();

        Task<Trip?> GetByIdAsync(long id);

        Task AddAsync(Trip trip);

        Task UpdateAsync(Trip trip);

        Task DeleteAsync(long id);

        Task<IEnumerable<Trip>> SearchAsync(string? searchTerm);

        Task<TripDetailsDTO?> GetDetailsAsync(long id);
    }
}
