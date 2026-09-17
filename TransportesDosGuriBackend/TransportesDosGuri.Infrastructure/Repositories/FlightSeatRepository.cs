using Dapper;
using TransportesDosGuri.Core.Domain.Entities;
using TransportesDosGuri.Core.Domain.RepositoryContracts;
using TransportesDosGuri.Infrastructure.Data;

namespace TransportesDosGuri.Infrastructure.Repositories
{
    public class FlightSeatRepository : IFlightSeatRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public FlightSeatRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AddAsync(FlightSeat flightSeat)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                INSERT INTO FlightSeat
                (
                    FlightId,
                    AircraftId,
                    SeatNumber,
                    Class,
                    Location,
                    Side,
                    Status
                )
                OUTPUT INSERTED.Id
                VALUES
                (
                    @FlightId,
                    @AircraftId,
                    @SeatNumber,
                    @Class,
                    @Location,
                    @Side,
                    @Status
                );
                """;

            var parameters = new DynamicParameters();
            parameters.Add("FlightId", flightSeat.FlightId);
            parameters.Add("AircraftId", flightSeat.AircraftId);
            parameters.Add("SeatNumber", flightSeat.SeatNumber);
            parameters.Add("Class", (int)flightSeat.Class);
            parameters.Add("Location", (int)flightSeat.Location);
            parameters.Add("Side", (int)flightSeat.Side);
            parameters.Add("Status", (int)flightSeat.Status);

            flightSeat.Id = await connection.ExecuteScalarAsync<long>(sql, parameters);
        }

        public async Task DeleteAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                DELETE FROM FlightSeat
                WHERE Id = @Id;
                """;

            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<FlightSeat>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                SELECT
                    Id,
                    FlightId,
                    AircraftId,
                    SeatNumber,
                    Class,
                    Location,
                    Side,
                    Status
                FROM FlightSeat
                ORDER BY Id DESC;
                """;

            return await connection.QueryAsync<FlightSeat>(sql);
        }

        public async Task<FlightSeat?> GetByIdAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                SELECT
                    Id,
                    FlightId,
                    AircraftId,
                    SeatNumber,
                    Class,
                    Location,
                    Side,
                    Status
                FROM FlightSeat
                WHERE Id = @Id;
                """;

            return await connection.QueryFirstOrDefaultAsync<FlightSeat>(sql, new { Id = id });
        }

        public async Task UpdateAsync(FlightSeat flightSeat)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                UPDATE FlightSeat
                SET
                    FlightId = @FlightId,
                    AircraftId = @AircraftId,
                    SeatNumber = @SeatNumber,
                    Class = @Class,
                    Location = @Location,
                    Side = @Side,
                    Status = @Status
                WHERE Id = @Id;
                """;

            await connection.ExecuteAsync(sql, flightSeat);
        }

        public async Task<bool> TryReserveAsync(long flightSeatId, long flightId)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                UPDATE FlightSeat
                SET
                    Status = @ReservedStatus
                WHERE Id = @FlightSeatId
                AND FlightId = @FlightId
                AND Status = @AvailableStatus;
                """;

            var rows = await connection.ExecuteAsync(
                sql,
                new
                {
                    FlightSeatId = flightSeatId,
                    FlightId = flightId,
                    ReservedStatus = (int)FlightSeatStatus.Reservado,
                    AvailableStatus = (int)FlightSeatStatus.Disponível
                });

            return rows == 1;
        }

        public async Task<IEnumerable<FlightSeat>> GetByFlightIdsAsync(IEnumerable<long> flightIds)
        {
            using var connection = _connectionFactory.CreateConnection();

            if (flightIds == null || !flightIds.Any())
            {
                return Enumerable.Empty<FlightSeat>();
            }

            const string sql = @"
                SELECT 
                    id AS Id,
                    aircraft_id AS AircraftId,
                    flight_id AS FlightId,
                    seat_number AS SeatNumber,
                    class AS Class,
                    location AS Location,
                    side AS Side,
                    status AS Status
                FROM flight_seats
                WHERE flight_id IN @FlightIds;";
            return await connection.QueryAsync<FlightSeat>(sql, new { FlightIds = flightIds });
        }
    }
}

