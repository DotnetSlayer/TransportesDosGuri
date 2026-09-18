using Dapper;
using System.Data;
using TransportesDosGuri.Core.Application.DTOs;
using TransportesDosGuri.Core.Application.DTOs.QuestPDF;
using TransportesDosGuri.Core.Domain.Entities;
using TransportesDosGuri.Core.Domain.RepositoryContracts;
using TransportesDosGuri.Infrastructure.Data;

namespace TransportesDosGuri.Infrastructure.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PurchaseRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task AddAsync(Purchase purchase)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                INSERT INTO Purchase
                (
                    ApplicationUserId,
                    TripId,
                    PurchaseDate,
                    PurchasePrice,
                    Status,
                    AsaasPaymentId,
                    AsaasInvoiceUrl
                )
                OUTPUT INSERTED.Id
                VALUES
                (
                    @ApplicationUserId,
                    @TripId,
                    @PurchaseDate,
                    @PurchasePrice,
                    @Status,
                    @AsaasPaymentId,
                    @AsaasInvoiceUrl
                );
                """;

            purchase.Id = await connection.ExecuteScalarAsync<long>(sql, purchase);
        }

        public async Task DeleteAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                DELETE FROM Purchase
                WHERE Id = @Id;
                """;

            await connection.ExecuteAsync(sql, new { Id = id });
        }

        public async Task<IEnumerable<Purchase>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                SELECT
                    Id,
                    ApplicationUserId,
                    TripId,
                    PurchaseDate,
                    PurchasePrice,
                    Status,
                    AsaasPaymentId,
                    AsaasInvoiceUrl
                FROM Purchase
                ORDER BY Id DESC;
                """;

            return await connection.QueryAsync<Purchase>(sql);
        }

        public async Task<Purchase?> GetByIdAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                SELECT
                    Id,
                    ApplicationUserId,
                    TripId,
                    PurchaseDate,
                    PurchasePrice,
                    Status,
                    AsaasPaymentId,
                    AsaasInvoiceUrl
                FROM Purchase
                WHERE Id = @Id;
                """;

            return await connection.QueryFirstOrDefaultAsync<Purchase>(sql, new { Id = id });
        }

        public async Task UpdateAsync(Purchase purchase)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                UPDATE Purchase
                SET
                    ApplicationUserId = @ApplicationUserId,
                    TripId = @TripId,
                    PurchaseDate = @PurchaseDate,
                    PurchasePrice = @PurchasePrice,
                    Status = @Status,
                    AsaasPaymentId = @AsaasPaymentId,
                    AsaasInvoiceUrl = @AsaasInvoiceUrl
                WHERE Id = @Id;
                """;

            await connection.ExecuteAsync(sql, purchase);
        }

        public async Task<PurchaseResultDTO> CreatePurchaseAsync(long userId, BuySeatRequestDTO request)
        {
            if (request == null)
                throw new ArgumentException("Requisição Inválida!");

            if (request.TripId <= 0)
                throw new ArgumentException("Requisição Inválida!");

            if (request.FlightSeatIds == null || !request.FlightSeatIds.Any())
                throw new ArgumentException("Requisição Inválida!");

            var seatIds = request.FlightSeatIds
                .Distinct()
                .ToArray();

            using var connection = _connectionFactory.CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction(
                IsolationLevel.Serializable);

            try
            {
                const string sqlSeats = """
                    SELECT
                        fs.Id,
                        fs.FlightId,
                        fs.AircraftId,
                        fs.SeatNumber,
                        fs.Class,
                        fs.Location,
                        fs.Side,
                        fs.Status,
                        f.TripId,
                        t.TotalPrice AS TripTotalPrice
                    FROM FlightSeat fs WITH (UPDLOCK, HOLDLOCK)
                    INNER JOIN Flight f
                        ON f.Id = fs.FlightId
                    INNER JOIN Trip t
                        ON t.Id = f.TripId
                    WHERE fs.Id IN @SeatIds
                    """;

                var seats = (
                    await connection.QueryAsync<PurchaseSeatRow>(
                        sqlSeats,
                        new { SeatIds = seatIds },
                        transaction)
                ).ToList();

                if (seats.Count != seatIds.Length)
                    throw new InvalidOperationException("Requisição Inválida!");

                if (seats.Any(x => x.TripId != request.TripId))
                    throw new InvalidOperationException("Requisição Inválida!");

                if (seats.Any(x => x.Status != FlightSeatStatus.Disponível))
                    throw new InvalidOperationException("Requisição Inválida!");

                var totalPrice = seats.Sum(seat =>
                {
                    var adicional = seat.Class switch
                    {
                        SeatClass.Executivo => 100m,
                        SeatClass.Premium => 200m,
                        _ => 0m
                    };

                    return seat.TripTotalPrice + adicional;
                });

                const string sqlPurchase = """
                    INSERT INTO Purchase
                    (
                        ApplicationUserId,
                        TripId,
                        PurchaseDate,
                        PurchasePrice,
                        Status
                    )
                    OUTPUT INSERTED.Id
                    VALUES
                    (
                        @ApplicationUserId,
                        @TripId,
                        @PurchaseDate,
                        @PurchasePrice,
                        @Status
                    )
                    """;

                var purchaseId = await connection.ExecuteScalarAsync<long>(
                    sqlPurchase,
                    new
                    {
                        ApplicationUserId = userId,
                        TripId = request.TripId,
                        PurchaseDate = DateTime.Now,
                        PurchasePrice = totalPrice,
                        Status = PurchaseStatus.Pendente
                    },
                    transaction);

                const string sqlUpdateSeats = """
                    UPDATE FlightSeat
                    SET Status = @ReservedStatus
                    WHERE Id IN @SeatIds
                      AND Status = @AvailableStatus
                    """;

                var updatedSeats = await connection.ExecuteAsync(
                    sqlUpdateSeats,
                    new
                    {
                        SeatIds = seatIds,
                        ReservedStatus = FlightSeatStatus.Reservado,
                        AvailableStatus = FlightSeatStatus.Disponível
                    },
                    transaction);

                if (updatedSeats != seatIds.Length)
                    throw new InvalidOperationException("Requisição Inválida!");

                const string sqlReservation = """
                    INSERT INTO Reservation
                    (
                        ApplicationUserId,
                        FlightSeatId,
                        PurchaseId,
                        ReservationDate,
                        Status,
                        Price
                    )
                    VALUES
                    (
                        @ApplicationUserId,
                        @FlightSeatId,
                        @PurchaseId,
                        @ReservationDate,
                        @Status,
                        @Price
                    )
                    """;

                foreach (var seat in seats)
                {
                    var adicional = seat.Class switch
                    {
                        SeatClass.Executivo => 100m,
                        SeatClass.Premium => 200m,
                        _ => 0m
                    };

                    await connection.ExecuteAsync(
                        sqlReservation,
                        new
                        {
                            ApplicationUserId = userId,
                            FlightSeatId = seat.Id,
                            PurchaseId = purchaseId,
                            ReservationDate = DateTime.Now,
                            Status = ReservationStatus.Pendente,
                            Price = seat.TripTotalPrice + adicional
                        },
                        transaction);
                }

                transaction.Commit();

                return new PurchaseResultDTO
                {
                    PurchaseId = purchaseId,
                    TotalPrice = totalPrice,
                    Status = PurchaseStatus.Pendente,
                    Message = "Compra criada com sucesso. Aguardando pagamento."
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<PurchaseDetailsDTO?> GetUserPurchaseAsync(long purchaseId, long userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sqlPurchase = """
                SELECT
                    p.Id,
                    p.ApplicationUserId,
                    p.TripId,
                    t.TripName,
                    p.PurchaseDate,
                    p.PurchasePrice,
                    p.Status,
                    p.AsaasPaymentId,
                    p.AsaasInvoiceUrl
                FROM Purchase p
                INNER JOIN Trip t
                    ON t.Id = p.TripId
                WHERE p.Id = @PurchaseId
                  AND p.ApplicationUserId = @UserId
                """;

            var purchase = await connection.QueryFirstOrDefaultAsync<PurchaseDetailsDTO>(
                sqlPurchase,
                new { PurchaseId = purchaseId, UserId = userId });

            if (purchase == null)
                return null;

            const string sqlReservations = """
                SELECT
                    r.Id,
                    r.ApplicationUserId,
                    r.FlightSeatId,
                    r.PurchaseId,
                    r.ReservationDate,
                    r.Status,
                    r.Price
                FROM Reservation r
                WHERE r.PurchaseId = @PurchaseId
                  AND r.ApplicationUserId = @UserId
                ORDER BY r.Id
                """;

            var reservations = await connection.QueryAsync<ReservationDTO>(
                sqlReservations,
                new { PurchaseId = purchaseId, UserId = userId });

            purchase.Reservations = reservations.ToList();

            return purchase;
        }

        public async Task<bool> ProcessPaymentAsync(long purchaseId, long userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction(
                IsolationLevel.Serializable);

            try
            {
                const string sqlPurchase = """
                    SELECT
                        Id,
                        ApplicationUserId,
                        TripId,
                        PurchaseDate,
                        PurchasePrice,
                        Status
                    FROM Purchase WITH (UPDLOCK, HOLDLOCK)
                    WHERE Id = @PurchaseId
                      AND ApplicationUserId = @UserId
                    """;

                var purchase = await connection.QueryFirstOrDefaultAsync<Purchase>(
                    sqlPurchase,
                    new { PurchaseId = purchaseId, UserId = userId },
                    transaction);

                if (purchase == null)
                {
                    transaction.Rollback();
                    return false;
                }

                if (purchase.Status != PurchaseStatus.Pendente)
                {
                    transaction.Rollback();
                    return false;
                }

                const string sqlUpdatePurchase = """
                    UPDATE Purchase
                    SET Status = @Status
                    WHERE Id = @PurchaseId
                      AND ApplicationUserId = @UserId
                    """;

                await connection.ExecuteAsync(
                    sqlUpdatePurchase,
                    new
                    {
                        Status = PurchaseStatus.Confirmado,
                        PurchaseId = purchaseId,
                        UserId = userId
                    },
                    transaction);

                const string sqlUpdateReservations = """
                    UPDATE Reservation
                    SET Status = @Status
                    WHERE PurchaseId = @PurchaseId
                      AND ApplicationUserId = @UserId
                    """;

                await connection.ExecuteAsync(
                    sqlUpdateReservations,
                    new
                    {
                        Status = ReservationStatus.Confirmado,
                        PurchaseId = purchaseId,
                        UserId = userId
                    },
                    transaction);

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<PurchaseDetailsDTO>> GetMyTripsAsync(long userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sqlPurchases = """
                SELECT
                    p.Id,
                    p.ApplicationUserId,
                    p.TripId,
                    t.TripName,
                    p.PurchaseDate,
                    p.PurchasePrice,
                    p.Status
                FROM Purchase p
                INNER JOIN Trip t
                    ON t.Id = p.TripId
                WHERE p.ApplicationUserId = @UserId
                  AND p.Status = @Status
                ORDER BY p.PurchaseDate DESC
                """;

            var purchases = (
                await connection.QueryAsync<PurchaseDetailsDTO>(
                    sqlPurchases,
                    new { UserId = userId, Status = PurchaseStatus.Confirmado })
            ).ToList();

            if (!purchases.Any())
                return purchases;

            var purchaseIds = purchases.Select(x => x.Id).ToArray();

            const string sqlReservations = """
                SELECT
                    r.Id,
                    r.ApplicationUserId,
                    r.FlightSeatId,
                    r.PurchaseId,
                    r.ReservationDate,
                    r.Status,
                    r.Price
                FROM Reservation r
                WHERE r.ApplicationUserId = @UserId
                  AND r.PurchaseId IN @PurchaseIds
                ORDER BY r.Id
                """;

            var reservations = (
                await connection.QueryAsync<ReservationDTO>(
                    sqlReservations,
                    new { UserId = userId, PurchaseIds = purchaseIds })
            ).ToList();

            foreach (var purchase in purchases)
            {
                purchase.Reservations = reservations
                    .Where(r => r.PurchaseId == purchase.Id)
                    .ToList();
            }

            return purchases;
        }

        public async Task<ReceiptDTO?> GetReceiptDataAsync(long purchaseId, long userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sqlPurchase = """
                SELECT
                    p.Id AS PurchaseId,
                    p.PurchaseDate,
                    p.PurchasePrice AS TotalPrice,
                    p.Status,
                    u.Name AS CustomerName,
                    u.Email AS CustomerEmail,
                    u.IdentityNumber AS CustomerIdentityNumber,
                    t.Id AS TripId,
                    t.TripName,
                    ao.Name AS OriginAirport,
                    ao.City AS OriginCity,
                    ad.Name AS DestinyAirport,
                    ad.City AS DestinyCity,
                    t.DepartureTime AS TripDepartureTime,
                    t.ArrivalTime AS TripArrivalTime
                FROM Purchase p
                INNER JOIN AspNetUsers u ON u.Id = p.ApplicationUserId
                INNER JOIN Trip t ON t.Id = p.TripId
                INNER JOIN Airport ao ON ao.Id = t.OriginAirportId
                INNER JOIN Airport ad ON ad.Id = t.DestinyAirportId
                WHERE p.Id = @PurchaseId
                  AND p.ApplicationUserId = @UserId
                  AND p.Status = @ConfirmedStatus
                """;

            var purchase = await connection.QueryFirstOrDefaultAsync<ReceiptDTO>(
                sqlPurchase,
                new
                {
                    PurchaseId = purchaseId,
                    UserId = userId,
                    ConfirmedStatus = (int)PurchaseStatus.Confirmado
                });

            if (purchase == null)
                return null;

            const string sqlReservations = """
                SELECT
                    r.Id AS ReservationId,
                    r.Price,
                    fs.SeatNumber,
                    fs.Class AS SeatClass,
                    fs.Location AS SeatLocation,
                    fs.Side AS SeatSide,
                    f.Id AS FlightId,
                    ac.Model AS AircraftModel,
                    f.DepartureTime AS FlightDepartureTime,
                    f.ArrivalTime AS FlightArrivalTime,
                    ao.Name AS FlightOriginAirport,
                    ad.Name AS FlightDestinyAirport
                FROM Reservation r
                INNER JOIN FlightSeat fs ON fs.Id = r.FlightSeatId
                INNER JOIN Flight f ON f.Id = fs.FlightId
                INNER JOIN Aircraft ac ON ac.Id = f.AircraftId
                INNER JOIN Airport ao ON ao.Id = f.OriginAirportId
                INNER JOIN Airport ad ON ad.Id = f.DestinyAirportId
                WHERE r.PurchaseId = @PurchaseId
                  AND r.ApplicationUserId = @UserId
                ORDER BY r.Id
                """;

            var reservations = await connection.QueryAsync<ReceiptReservationDTO>(
                sqlReservations,
                new { PurchaseId = purchaseId, UserId = userId });

            purchase.Reservations = reservations.ToList();

            return purchase;
        }

        // ============================================================
        // NOVO — Corrigido: tabela é "Purchase" (singular), não "Purchases"
        // ============================================================
        public async Task UpdateAsaasPaymentIdAsync(long purchaseId, string asaasPaymentId, string? invoiceUrl)
        {
            const string sql = """
                UPDATE Purchase
                SET AsaasPaymentId  = @AsaasPaymentId,
                    AsaasInvoiceUrl = @AsaasInvoiceUrl
                WHERE Id = @Id
                """;

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new
            {
                Id = purchaseId,
                AsaasPaymentId = asaasPaymentId,
                AsaasInvoiceUrl = invoiceUrl
            });
        }

        private class PurchaseSeatRow
        {
            public long Id { get; set; }
            public long FlightId { get; set; }
            public long AircraftId { get; set; }
            public string SeatNumber { get; set; } = string.Empty;
            public SeatClass Class { get; set; }
            public SeatLocation Location { get; set; }
            public SeatSide Side { get; set; }
            public FlightSeatStatus Status { get; set; }
            public long TripId { get; set; }
            public decimal TripTotalPrice { get; set; }
        }
    }
}