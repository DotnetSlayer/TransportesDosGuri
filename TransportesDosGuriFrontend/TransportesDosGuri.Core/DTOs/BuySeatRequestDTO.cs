using System;
using System.Collections.Generic;
using System.Text;

namespace TransportesDosGuri.Core.DTOs
{
    public class BuySeatRequestDTO
    {
        public long TripId { get; set; }

        public List<long> FlightSeatIds { get; set; } = new();
    }
}
