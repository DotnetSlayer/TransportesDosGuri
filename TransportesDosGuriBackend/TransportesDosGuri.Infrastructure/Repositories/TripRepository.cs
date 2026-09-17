using Dapper;
using TransportesDosGuri.Core.Application.DTOs;
using TransportesDosGuri.Core.Domain.Entities;
using TransportesDosGuri.Core.Domain.RepositoryContracts;
using TransportesDosGuri.Infrastructure.Data;

namespace TransportesDosGuri.Infrastructure.Repositories
{
    public class TripRepository : ITripRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TripRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AddAsync(Trip trip)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                INSERT INTO Trip
                (
                    TripName,
                    OriginAirportId,
                    DestinyAirportId,
                    DepartureTime,
                    ArrivalTime,
                    TotalPrice
                )
                OUTPUT INSERTED.Id
                VALUES
                (
                    @TripName,
                    @OriginAirportId,
                    @DestinyAirportId,
                    @DepartureTime,
                    @ArrivalTime,
                    @TotalPrice
                );
                """;

            trip.Id = await connection.ExecuteScalarAsync<long>(sql, trip);
        }

        public async Task DeleteAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                DELETE FROM Trip
                WHERE Id = @Id;
                """;

            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<Trip>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                SELECT
                    Id,
                    TripName,
                    OriginAirportId,
                    DestinyAirportId,
                    DepartureTime,
                    ArrivalTime,
                    TotalPrice
                FROM Trip
                ORDER BY Id DESC;
                """;

            return await connection.QueryAsync<Trip>(sql);
        }

        public async Task<Trip?> GetByIdAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                SELECT
                    Id,
                    TripName,
                    OriginAirportId,
                    DestinyAirportId,
                    DepartureTime,
                    ArrivalTime,
                    TotalPrice
                FROM Trip
                WHERE Id = @Id;
                """;

            return await connection.QueryFirstOrDefaultAsync<Trip>(sql, new { Id = id });
        }

        public async Task UpdateAsync(Trip trip)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                UPDATE Trip
                SET
                    TripName = @TripName,
                    OriginAirportId = @OriginAirportId,
                    DestinyAirportId = @DestinyAirportId,
                    DepartureTime = @DepartureTime,
                    ArrivalTime = @ArrivalTime,
                    TotalPrice = @TotalPrice
                WHERE Id = @Id;
                """;

            await connection.ExecuteAsync(sql, trip);
        }

        public async Task<IEnumerable<Trip>> SearchAsync(string? searchTerm)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            const string sql = """
                SELECT
                    t.Id,
                    t.TripName,
                    t.OriginAirportId,
                    t.DestinyAirportId,
                    t.DepartureTime,
                    t.ArrivalTime,
                    t.TotalPrice
                FROM Trip t
                INNER JOIN Airport ao
                    ON ao.Id = t.OriginAirportId
                INNER JOIN Airport ad
                    ON ad.Id = t.DestinyAirportId
                WHERE
                    @SearchTerm IS NULL
                    OR LTRIM(RTRIM(@SearchTerm)) = ''
                    OR t.TripName LIKE '%' + @SearchTerm + '%'
                    OR ao.Name LIKE '%' + @SearchTerm + '%'
                    OR ao.City LIKE '%' + @SearchTerm + '%'
                    OR ad.Name LIKE '%' + @SearchTerm + '%'
                    OR ad.City LIKE '%' + @SearchTerm + '%'
                ORDER BY t.DepartureTime
                """;

            return await connection.QueryAsync<Trip>(
                sql,
                new
                {
                    SearchTerm = searchTerm
                });
        }

        public async Task<TripDetailsDTO?> GetDetailsAsync(long id)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            const string sqlTrip = """
                SELECT
                    Id,
                    TripName,
                    OriginAirportId,
                    DestinyAirportId,
                    DepartureTime,
                    ArrivalTime,
                    TotalPrice
                FROM Trip
                WHERE Id = @Id
                """;

            var trip = await connection.QueryFirstOrDefaultAsync<TripDetailsDTO>(
                sqlTrip,
                new { Id = id });

            if (trip == null)
                return null;

            const string sqlFlights = """
                SELECT
                    Id,
                    AircraftId,
                    OriginAirportId,
                    DestinyAirportId,
                    DepartureTime,
                    ArrivalTime,
                    BasePrice
                FROM Flight
                WHERE TripId = @TripId
                ORDER BY DepartureTime
                """;

            var flights = (
                await connection.QueryAsync<FlightDetailsDTO>(
                    sqlFlights,
                    new
                    {
                        TripId = id
                    })
            ).ToList();

            const string sqlSeats = """
                SELECT
                    fs.Id,
                    fs.FlightId,
                    fs.AircraftId,
                    fs.SeatNumber,
                    fs.Class,
                    fs.Location,
                    fs.Side,
                    fs.Status
                FROM FlightSeat fs
                INNER JOIN Flight f
                    ON f.Id = fs.FlightId
                WHERE f.TripId = @TripId
                ORDER BY fs.FlightId, fs.SeatNumber
                """;

            var seats = (
                await connection.QueryAsync<FlightSeatDTO>(
                    sqlSeats,
                    new
                    {
                        TripId = id
                    })
            ).ToList();

            foreach (var flight in flights)
            {
                flight.Seats = seats
                    .Where(x => x.FlightId == flight.Id)
                    .ToList();
            }

            trip.Flights = flights;

            return trip;
        }
    }
}
