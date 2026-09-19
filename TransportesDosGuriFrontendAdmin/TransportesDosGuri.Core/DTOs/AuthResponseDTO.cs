using System.Text.Json.Serialization;

namespace TransportesDosGuri.Core.DTOs
{
    public class AuthResponseDTO
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("token")]
        public TokenPayloadDTO? TokenData { get; set; }
    }

    public class TokenPayloadDTO
    {
        [JsonPropertyName("personName")]
        public string? PersonName { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("token")]
        public string? JwtToken { get; set; }

        [JsonPropertyName("tokenExpiration")]
        public DateTime TokenExpiration { get; set; }

        [JsonPropertyName("refreshToken")]
        public string? RefreshToken { get; set; }

        [JsonPropertyName("refreshTokenExpirationDateTime")]
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
