namespace TransportesDosGuri.Core.Domain.Entities
{
    public class Seat
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
        Econômico,

        Executivo,

        Premium
    }

    public enum SeatLocation
    {
        Janela,

        Corredor,

        Meio
    }

    public enum SeatSide
    {
        Esquerdo,

        Meio,

        Direito
    }
}
