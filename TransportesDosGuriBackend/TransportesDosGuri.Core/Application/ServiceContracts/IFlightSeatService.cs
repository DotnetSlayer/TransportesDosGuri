using TransportesDosGuri.Core.Application.DTOs;

namespace TransportesDosGuri.Core.Application.ServiceContracts
{
    public interface IFlightSeatService
    {
        Task<IEnumerable<FlightSeatDTO>> GetAllAsync();

        Task<FlightSeatDTO?> GetByIdAsync(long id);

        Task<FlightSeatDTO> CreateAsync(FlightSeatDTO flightSeat);

        Task<bool> UpdateAsync(long id, FlightSeatDTO flightSeat);

        Task<bool> DeleteAsync(long id);

        Task<IEnumerable<FlightSeatDTO>> GetByFlightIdsAsync(IEnumerable<long> flightIds);
    }
}