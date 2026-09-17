using System.Net.Http.Json;
using System.Text.Json;
using TransportesDosGuri.Core.Application.DTOs.Asaas;
using TransportesDosGuri.Core.Domain.RepositoryContracts;

namespace TransportesDosGuri.Infrastructure.ExternalServices.Asaas
{
    public class AsaasClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAsaasIntegrationRepository _integrationRepository;

        public AsaasClient(
            IHttpClientFactory httpClientFactory,
            IAsaasIntegrationRepository integrationRepository)
        {
            _httpClientFactory = httpClientFactory;
            _integrationRepository = integrationRepository;
        }

        private async Task<HttpClient> CreateConfiguredClientAsync()
        {
            var config = await _integrationRepository.GetActiveAsync();

            if (config == null || string.IsNullOrWhiteSpace(config.ApiKey) || string.IsNullOrWhiteSpace(config.BaseUrl))
            {
                throw new InvalidOperationException("Requisição Inválida!");
            }

            var client = _httpClientFactory.CreateClient("AsaasClient");
            client.BaseAddress = new Uri(config.BaseUrl);

            client.DefaultRequestHeaders.Remove("access_token");
            client.DefaultRequestHeaders.Add("access_token", config.ApiKey);

            return client;
        }

        public async Task<string> CreateCustomerAsync(AsaasCustomerRequestDTO request)
        {
            var client = await CreateConfiguredClientAsync();

            var body = new
            {
                name = request.Name,
                email = request.Email,
                mobilePhone = request.MobilePhone,
                cpfCnpj = request.CpfCnpj
            };

            var response = await client.PostAsJsonAsync("customers", body);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Requisição Inválida!: {content}");

            using var doc = JsonDocument.Parse(content);
            return doc.RootElement.GetProperty("id").GetString()!;
        }

        public async Task<string> CreateSinglePaymentAsync(AsaasPaymentRequestDTO request)
        {
            var client = await CreateConfiguredClientAsync();

            var body = new
            {
                customer = request.Customer,
                billingType = string.IsNullOrWhiteSpace(request.BillingType) ? "BOLETO" : request.BillingType,
                value = request.Value,
                dueDate = request.DueDate,
                description = request.Description
            };

            var response = await client.PostAsJsonAsync("payments", body);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Requisição Inválida!: {content}");

            using var doc = JsonDocument.Parse(content);
            return doc.RootElement.GetProperty("id").GetString()!;
        }
    }
}