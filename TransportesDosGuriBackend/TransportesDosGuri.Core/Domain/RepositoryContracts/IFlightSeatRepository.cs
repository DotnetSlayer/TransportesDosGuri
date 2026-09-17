using TransportesDosGuri.Core.Domain.Entities;

namespace TransportesDosGuri.Core.Domain.RepositoryContracts
{
    public interface IFlightSeatRepository
    {
        Task<IEnumerable<FlightSeat>> GetAllAsync();

        Task<FlightSeat?> GetByIdAsync(long id);

        Task AddAsync(FlightSeat flightSeat);

        Task UpdateAsync(FlightSeat flightSeat);

        Task DeleteAsync(long id);

        Task<bool> TryReserveAsync(
            long flightSeatId,
            long flightId);

        Task<IEnumerable<FlightSeat>> GetByFlightIdsAsync(IEnumerable<long> flightIds);
    }
}
