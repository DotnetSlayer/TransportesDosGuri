namespace TransportesDosGuri.Core.Domain.Entities
{
    public class Reservation
    {
        public long Id { get; set; }

        public long ApplicationUserId { get; set; }

        public long FlightSeatId { get; set; }

        public long PurchaseId { get; set; }

        public DateTime ReservationDate { get; set; }

        public ReservationStatus Status { get; set; }

        public decimal Price { get; set; }
    }

    public enum ReservationStatus
    {
        Pendente,

        Confirmado,

        Cancelado
    }
}
