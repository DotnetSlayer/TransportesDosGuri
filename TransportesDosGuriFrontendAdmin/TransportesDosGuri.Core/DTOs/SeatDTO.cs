using System;
using System.Collections.Generic;
using System.Text;

namespace TransportesDosGuri.Core.DTOs
{
    public class SeatDTO
    {
        public long Id { get; set; }

        public long AircraftId { get; set; }

        public string? SeatNumber { get; set; }

        public SeatClass Class { get; set; }

        public SeatLocation Location { get; set; }

        public SeatSide Side { get; set; }


    }

    public enum SeatClass
    {
        Econômico = 0,

        Executivo = 1,

        Premium = 2
    }

    public enum SeatLocation
    {
        Janela = 0,

        Corredor = 1,

        Meio = 2
    }

    public enum SeatSide
    {
        Esquerdo = 0,

        Meio = 1,

        Direito = 2
    }
}
