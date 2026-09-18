using System;
using System.Collections.Generic;
using System.Text;
using TransportesDosGuri.Core.Enums;

namespace TransportesDosGuri.Core.DTOs
{
    public class FlightSeatDTO
    {
        public long Id { get; set; }

        public long FlightId { get; set; }

        public long AircraftId { get; set; }

        public string SeatNumber { get; set; } = string.Empty;

        public SeatClass Class { get; set; }

        public string Location { get; set; } = string.Empty;

        public string Side { get; set; } = string.Empty;

        public FlightSeatStatus Status { get; set; }
    }
}
