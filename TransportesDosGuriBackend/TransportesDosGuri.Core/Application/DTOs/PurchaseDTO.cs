using TransportesDosGuri.Core.Domain.Entities;

namespace TransportesDosGuri.Core.Application.DTOs
{
    public class PurchaseDTO
    {
        public long Id { get; set; }

        public long ApplicationUserId { get; set; }

        public long TripId { get; set; }

        public DateTime PurchaseDate { get; set; }

        public decimal PurchasePrice { get; set; }

        public PurchaseStatus Status { get; set; }

    }
}
