using TransportesDosGuri.Core.Enums;

namespace TransportesDosGuri.Core.DTOs
{
    public class PurchaseDetailsDTO
    {
        public long Id { get; set; }

        public long ApplicationUserId { get; set; }

        public long TripId { get; set; }

        public string TripName { get; set; } = string.Empty;

        public DateTime PurchaseDate { get; set; }

        public decimal PurchasePrice { get; set; }

        public PurchaseStatus Status { get; set; }

        public List<ReservationDetailsDTO> Reservations { get; set; } = new();
    }
}
