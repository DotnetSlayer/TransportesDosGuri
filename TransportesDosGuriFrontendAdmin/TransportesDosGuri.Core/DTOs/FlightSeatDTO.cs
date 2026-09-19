namespace TransportesDosGuri.Core.DTOs
{
    public class FlightSeatDTO
    {
        public long Id { get; set; }

        public long FlightId { get; set; }

        public long AircraftId { get; set; }

        public string? SeatNumber { get; set; }

        public SeatClass Class { get; set; }

        public SeatLocation Location { get; set; }

        public SeatSide Side { get; set; }

        public FlightSeatStatus Status { get; set; }
    }

    public enum FlightSeatStatus
    {
        Disponível = 0,
        Reservado = 1
    }
}
