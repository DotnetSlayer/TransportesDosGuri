using TransportesDosGuri.Core.Application.DTOs;
using TransportesDosGuri.Core.Application.ServiceContracts;
using TransportesDosGuri.Core.Domain.Entities;
using TransportesDosGuri.Core.Domain.RepositoryContracts;

namespace TransportesDosGuri.Core.Application.Services
{
    public class FlightSeatService : IFlightSeatService
    {
        private readonly IFlightSeatRepository _flightSeatRepository;

        public FlightSeatService(IFlightSeatRepository flightSeatRepository)
        {
            _flightSeatRepository = flightSeatRepository;
        }

        public async Task<FlightSeatDTO> CreateAsync(FlightSeatDTO flightSeat)
        {
            var flightSeatEntity = new FlightSeat
            {
                AircraftId = flightSeat.AircraftId,
                SeatNumber = flightSeat.SeatNumber,
                Class = flightSeat.Class,
                Location = flightSeat.Location,
                Side = flightSeat.Side,
                Status = flightSeat.Status,
                FlightId = flightSeat.FlightId
            };

            await _flightSeatRepository.AddAsync(flightSeatEntity);

            flightSeat.Id = flightSeatEntity.Id;

            return flightSeat;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var flightSeatEntity = await _flightSeatRepository.GetByIdAsync(id);

            if (flightSeatEntity == null)
            {
                return false;
            }

            await _flightSeatRepository.DeleteAsync(id);

            return true;
        }

        public async Task<IEnumerable<FlightSeatDTO>> GetAllAsync()
        {
            var flightSeatEntities = await _flightSeatRepository.GetAllAsync();

            return flightSeatEntities.Select(flightSeatEntities => new FlightSeatDTO
            {
                Id = flightSeatEntities.Id,
                AircraftId = flightSeatEntities.AircraftId,
                SeatNumber = flightSeatEntities.SeatNumber,
                Class = flightSeatEntities.Class,
                Location = flightSeatEntities.Location,
                Side = flightSeatEntities.Side,
                Status = flightSeatEntities.Status,
                FlightId = flightSeatEntities.FlightId
            });
        }

        public async Task<FlightSeatDTO?> GetByIdAsync(long id)
        {
            var flightSeatEntity = await _flightSeatRepository.GetByIdAsync(id);

            if (flightSeatEntity == null)
            {
                return null;
            }

            return new FlightSeatDTO
            {
                Id = flightSeatEntity.Id,
                AircraftId = flightSeatEntity.AircraftId,
                SeatNumber = flightSeatEntity.SeatNumber,
                Class = flightSeatEntity.Class,
                Location = flightSeatEntity.Location,
                Side = flightSeatEntity.Side,
                Status = flightSeatEntity.Status,
                FlightId = flightSeatEntity.FlightId
            };
        }

        public async Task<bool> UpdateAsync(long id, FlightSeatDTO flightSeat)
        {
            var existingFlightSeat = await _flightSeatRepository.GetByIdAsync(id);

            if (existingFlightSeat == null)
            {
                return false;
            }

            existingFlightSeat.AircraftId = flightSeat.AircraftId;
            existingFlightSeat.SeatNumber = flightSeat.SeatNumber;
            existingFlightSeat.Class = flightSeat.Class;
            existingFlightSeat.Location = flightSeat.Location;
            existingFlightSeat.Side = flightSeat.Side;
            existingFlightSeat.Status = flightSeat.Status;
            existingFlightSeat.FlightId = flightSeat.FlightId;

            await _flightSeatRepository.UpdateAsync(existingFlightSeat);

            return true;
        }

        public async Task<IEnumerable<FlightSeatDTO>> GetByFlightIdsAsync(IEnumerable<long> flightIds)
        {
            if (flightIds == null || !flightIds.Any())
            {
                return Enumerable.Empty<FlightSeatDTO>();
            }

            var flightSeatEntities = await _flightSeatRepository.GetByFlightIdsAsync(flightIds);

            return flightSeatEntities.Select(seat => new FlightSeatDTO
            {
                Id = seat.Id,
                AircraftId = seat.AircraftId,
                SeatNumber = seat.SeatNumber,
                Class = seat.Class,
                Location = seat.Location,
                Side = seat.Side,
                Status = seat.Status,
                FlightId = seat.FlightId
            });
        }
    }
}
