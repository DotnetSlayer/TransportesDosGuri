using System;
using System.Collections.Generic;
using System.Text;

namespace TransportesDosGuri.Core.DTOs
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

    public enum PurchaseStatus
    {
        Pendente = 0,

        Confirmado = 1,

        Cancelado = 2
    }
}
