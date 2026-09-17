using System;
using System.Collections.Generic;
using System.Text;

namespace TransportesDosGuri.Core.DTOs
{
    public class AirportDTO
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }
    }
}
