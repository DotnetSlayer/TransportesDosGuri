using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class AirportService : IAirportService
    {
        private readonly HttpClient _http;

        public AirportService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<AirportDTO>> GetAllAsync() =>
            await _http.GetFromJsonAsync<List<AirportDTO>>("/api/v1/Airport") ?? new();

        public async Task<AirportDTO?> GetByIdAsync(long id) =>
            await _http.GetFromJsonAsync<AirportDTO>($"/api/v1/Airport/{id}");

        public async Task<bool> CreateAsync(AirportDTO dto)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/Airport", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(long id, AirportDTO dto)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/Airport/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/Airport/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
