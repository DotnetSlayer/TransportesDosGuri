using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class AsaasIntegrationService : IAsaasIntegrationService
    {
        private readonly HttpClient _http;

        public AsaasIntegrationService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<AsaasIntegrationDTO>> GetAllAsync() =>
            await _http.GetFromJsonAsync<List<AsaasIntegrationDTO>>("/api/v1/AsaasIntegration") ?? new();

        public async Task<AsaasIntegrationDTO?> GetByIdAsync(long id) =>
            await _http.GetFromJsonAsync<AsaasIntegrationDTO>($"/api/v1/AsaasIntegration/{id}");

        public async Task<bool> CreateAsync(AsaasIntegrationDTO dto)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/AsaasIntegration", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(long id, AsaasIntegrationDTO dto)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/AsaasIntegration/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/AsaasIntegration/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
