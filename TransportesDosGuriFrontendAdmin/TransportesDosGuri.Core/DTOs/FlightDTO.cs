using System;
using System.Collections.Generic;
using System.Text;

namespace TransportesDosGuri.Core.DTOs
{
    public class FlightDTO
    {
        public long Id { get; set; }

        public long AircraftId { get; set; }
        public string? AircraftModel { get; set; } 

        public long OriginAirportId { get; set; }
        public string? OriginAirportName { get; set; } 

        public long DestinyAirportId { get; set; }
        public string? DestinyAirportName { get; set; }

        public DateTime DepartureTime { get; set; }

        public DateTime ArrivalTime { get; set; }

        public decimal BasePrice { get; set; }

        public long TripId { get; set; }
    }
}
