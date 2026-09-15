using System;
using System.Collections.Generic;
using System.Text;

namespace TransportesDosGuri.Core.DTOs
{
    public class AircraftDto
    {
        public long Id { get; set; }
        public AircraftType Type { get; set; } = AircraftType.Unspecified;
        public string? Model { get; set; }
    }

    public enum AircraftType
    {
        Unspecified = 0,
        CommercialAircraft,
        BusinessAircraft,
        CargoAircraft,
        RegionalAircraft,
        Jet
    }
}
