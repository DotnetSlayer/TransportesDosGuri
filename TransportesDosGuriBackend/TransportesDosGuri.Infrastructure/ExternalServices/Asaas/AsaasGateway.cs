using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TransportesDosGuri.Core.Domain.RepositoryContracts;
using TransportesDosGuri.Core.Domain.RepositoryContracts.Misc;

namespace TransportesDosGuri.Infrastructure.ExternalServices.Asaas
{
    public class AsaasGateway : IAsaasGateway
    {
        private readonly HttpClient _http;
        private readonly IAsaasIntegrationRepository _integrationRepo;

        public AsaasGateway(
            HttpClient http,
            IAsaasIntegrationRepository integrationRepo)
        {
            _http = http;
            _integrationRepo = integrationRepo;
        }

        // ============================================================
        // Monta o HttpRequestMessage com os headers obrigatórios
        // ============================================================
        private async Task<HttpRequestMessage> BuildRequestAsync(
            HttpMethod method,
            string relativeUrl,
            object? body = null)
        {
            var integration = await _integrationRepo.GetActiveAsync()
                ?? throw new InvalidOperationException("Integração Asaas não configurada.");

            var apiKey = integration.ApiKey?.Trim();
            var baseUrl = integration.BaseUrl?.Trim();

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("ApiKey do Asaas está vazia.");

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("BaseUrl do Asaas está vazia.");

            var url = baseUrl.TrimEnd('/') + "/" + relativeUrl.TrimStart('/');

            var request = new HttpRequestMessage(method, url);

            // Autenticação
            request.Headers.Add("access_token", apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Obrigatório pelo Asaas
            if (!request.Headers.Contains("User-Agent"))
                request.Headers.UserAgent.ParseAdd("TransportesDosGuri/1.0");

            if (body is not null)
                request.Content = JsonContent.Create(body);

            return request;
        }

        // ============================================================
        // Lê a resposta e joga o corpo do erro na exceção
        // ============================================================
        private static async Task<string> ReadSuccessOrThrowAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Asaas retornou {(int)response.StatusCode} {response.StatusCode}. " +
                    $"Corpo: {content}");
            }

            return content;
        }

        // ============================================================
        // POST /v3/customers
        // ============================================================
        public async Task<string> CreateCustomerAsync(
            string name,
            string email,
            string phone,
            string cpfCnpj)
        {
            using var request = await BuildRequestAsync(
                HttpMethod.Post,
                "v3/customers",
                new { name, email, phone, cpfCnpj });

            var response = await _http.SendAsync(request);
            var content = await ReadSuccessOrThrowAsync(response);

            // 🔍 Log temporário — remova depois
            Console.WriteLine("=== ASAAS CUSTOMER RESPONSE ===");
            Console.WriteLine(content);
            Console.WriteLine("===============================");

            var json = JsonSerializer.Deserialize<AsaasCustomerResponse>(content);

            if (json is null || string.IsNullOrWhiteSpace(json.Id))
                throw new InvalidOperationException(
                    $"Asaas não retornou o Id do customer. Corpo: {content}");

            return json.Id;
        }

        // ============================================================
        // POST /v3/payments
        // ============================================================
        public async Task<(string paymentId, string? invoiceUrl)> CreatePaymentAsync(
            string customerId,
            decimal value,
            DateTime dueDate,
            string description,
            string externalReference)
        {
            if (string.IsNullOrWhiteSpace(customerId))
                throw new InvalidOperationException(
                    "customerId está vazio — não é possível criar a cobrança.");

            using var request = await BuildRequestAsync(
                HttpMethod.Post,
                "v3/payments",
                new
                {
                    customer = customerId,
                    billingType = "PIX",
                    value,
                    dueDate = dueDate.ToString("yyyy-MM-dd"),
                    description,
                    externalReference
                });

            var response = await _http.SendAsync(request);
            var content = await ReadSuccessOrThrowAsync(response);

            // 🔍 Log temporário — remova depois
            Console.WriteLine("=== ASAAS PAYMENT RESPONSE ===");
            Console.WriteLine(content);
            Console.WriteLine("==============================");

            var json = JsonSerializer.Deserialize<AsaasPaymentResponse>(content);

            if (json is null || string.IsNullOrWhiteSpace(json.Id))
                throw new InvalidOperationException(
                    $"Asaas não retornou o Id do payment. Corpo: {content}");

            return (json.Id, json.InvoiceUrl);
        }

        // ============================================================
        // DTOs internos do Asaas (camelCase mapeado explicitamente)
        // ============================================================
        private class AsaasCustomerResponse
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;
        }

        private class AsaasPaymentResponse
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("invoiceUrl")]
            public string? InvoiceUrl { get; set; }
        }
    }
}