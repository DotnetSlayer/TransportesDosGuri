using System.Text.Json.Serialization;

namespace TransportesDosGuri.Core.DTOs
{
    public class ZipCodeResponseDTO
    {
        [JsonPropertyName("Address")]
        public string? Address { get; set; }

        [JsonPropertyName("District")]
        public string? District { get; set; }

        [JsonPropertyName("City")]
        public string? City { get; set; }

        [JsonPropertyName("State")]
        public string? State { get; set; }

        [JsonPropertyName("ZipCode")]
        public string? ZipCode { get; set; }

        [JsonPropertyName("Complement")]
        public string? Complement { get; set; }

        [JsonPropertyName("Ibge")]
        public string? Ibge { get; set; }
    }
}