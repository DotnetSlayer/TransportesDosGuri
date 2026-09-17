using System;
using System.Collections.Generic;
using System.Text;

namespace TransportesDosGuri.Core.Application.DTOs
{
    public class TripDetailsDTO
    {
        public long Id { get; set; }

        public string? TripName { get; set; }

        public long OriginAirportId { get; set; }

        public long DestinyAirportId { get; set; }

        public DateTime DepartureTime { get; set; }

        public DateTime ArrivalTime { get; set; }

        public decimal TotalPrice { get; set; }

        public List<FlightDetailsDTO> Flights { get; set; } = new();
    }
}
