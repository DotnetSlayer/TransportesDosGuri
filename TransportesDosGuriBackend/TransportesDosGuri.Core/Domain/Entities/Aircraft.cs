namespace TransportesDosGuri.Core.Domain.Entities
{
    public class Aircraft
    {
        public long Id { get; set; }

        public AircraftType Type { get; set; }

        public string? Model { get; set; }

    }

    public enum AircraftType
    {
        Indeterminado = 0,

        Comercial,

        Executiva,

        Cargueira,

        Regional,

        Jato
    }
}
