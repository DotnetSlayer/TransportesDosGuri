using System;
using System.Collections.Generic;
using System.Text;

namespace TransportesDosGuri.Core.DTOs
{
    public class ReservationDTO
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
        Pending,

        Confirmed,

        Canceled
    }
}
