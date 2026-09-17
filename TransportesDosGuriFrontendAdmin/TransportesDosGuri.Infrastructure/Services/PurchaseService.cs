using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly HttpClient _http;

        public PurchaseService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<PurchaseDTO>> GetAllAsync() =>
            await _http.GetFromJsonAsync<List<PurchaseDTO>>("/api/v1/Purchase") ?? new();

        public async Task<PurchaseDTO?> GetByIdAsync(long id) =>
            await _http.GetFromJsonAsync<PurchaseDTO>($"/api/v1/Purchase/{id}");

        public async Task<bool> CreateAsync(PurchaseDTO dto)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/Purchase", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(long id, PurchaseDTO dto)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/Purchase/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/Purchase/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
