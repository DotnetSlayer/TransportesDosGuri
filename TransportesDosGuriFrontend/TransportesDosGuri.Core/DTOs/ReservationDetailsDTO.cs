using TransportesDosGuri.Core.Enums;

namespace TransportesDosGuri.Core.DTOs
{
    public class ReservationDetailsDTO
    {
        public long Id { get; set; }

        public long PurchaseId { get; set; }

        public long FlightSeatId { get; set; }

        public string SeatNumber { get; set; } = string.Empty;

        public SeatClass Class { get; set; }

        public FlightSeatStatus SeatStatus { get; set; }

        public ReservationStatus Status { get; set; }

        public long FlightId { get; set; }

        public DateTime DepartureTime { get; set; }

        public DateTime ArrivalTime { get; set; }
    }
}
