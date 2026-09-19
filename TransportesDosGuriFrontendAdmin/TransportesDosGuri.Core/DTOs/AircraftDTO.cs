namespace TransportesDosGuri.Core.DTOs
{
    public class AircraftDTO
    {
        public long Id { get; set; }
        public AircraftType Type { get; set; } = AircraftType.Indeterminado;
        public string? Model { get; set; }
    }

    public enum AircraftType
    {
        Indeterminado = 0,

        Comercial = 1,

        Executiva = 2,

        Cargueira = 3,

        Regional = 4,

        Jato = 5
    }
}
