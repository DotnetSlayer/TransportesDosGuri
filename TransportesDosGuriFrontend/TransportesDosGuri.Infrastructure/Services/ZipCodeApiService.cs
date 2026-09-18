using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TransportesDosGuri.Core.DTOs;

namespace TransportesDosGuri.Infrastructure.Services
{
    public enum ZipCodeStatus
    {
        Success,
        InvalidFormat,
        NotFound,
        NetworkError,
        ServerError
    }

    public record ZipCodeResult(
        ZipCodeStatus Status,
        ZipCodeResponseDTO? Data = null,
        string? Message = null);

    public class ZipCodeApiService
    {
        private readonly HttpClient _http;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = null
        };

        public ZipCodeApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ZipCodeResult> GetAsync(string? code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return new ZipCodeResult(ZipCodeStatus.InvalidFormat, Message: "Informe um CEP.");

            var digits = new string(code.Where(char.IsDigit).ToArray());

            if (digits.Length != 8)
                return new ZipCodeResult(ZipCodeStatus.InvalidFormat, Message: "CEP deve ter 8 dígitos.");

            try
            {
                var response = await _http.GetAsync($"/api/v1/ViaCep/ZipCode/{digits}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return new ZipCodeResult(ZipCodeStatus.NotFound, Message: "CEP não encontrado.");

                if (!response.IsSuccessStatusCode)
                    return new ZipCodeResult(
                        ZipCodeStatus.ServerError,
                        Message: $"Erro do servidor ({(int)response.StatusCode}).");

                var raw = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(raw) || raw.Trim() == "{}")
                    return new ZipCodeResult(ZipCodeStatus.NotFound, Message: "CEP não encontrado.");

                ZipCodeResponseDTO? data;
                try
                {
                    data = JsonSerializer.Deserialize<ZipCodeResponseDTO>(raw, JsonOptions);
                }
                catch (JsonException ex)
                {
                    return new ZipCodeResult(
                        ZipCodeStatus.ServerError,
                        Message: $"Resposta inválida do servidor: {ex.Message}");
                }

                if (data is null || string.IsNullOrWhiteSpace(data.City))
                    return new ZipCodeResult(ZipCodeStatus.NotFound, Message: "CEP não encontrado.");

                return new ZipCodeResult(ZipCodeStatus.Success, data);
            }
            catch (HttpRequestException ex)
            {
                return new ZipCodeResult(
                    ZipCodeStatus.NetworkError,
                    Message: $"Falha de conexão: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                return new ZipCodeResult(
                    ZipCodeStatus.NetworkError,
                    Message: "Tempo esgotado ao consultar o CEP.");
            }
        }
    }
}