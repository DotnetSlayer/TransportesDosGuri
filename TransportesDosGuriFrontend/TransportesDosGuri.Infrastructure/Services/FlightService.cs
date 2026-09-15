using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class FlightService : IFlightService
    {
        private readonly HttpClient _http;

        public FlightService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<FlightDTO>> GetAllAsync() =>
            await _http.GetFromJsonAsync<List<FlightDTO>>("/api/v1/Flight") ?? new();

        public async Task<FlightDTO?> GetByIdAsync(long id) =>
            await _http.GetFromJsonAsync<FlightDTO>($"/api/v1/Flight/{id}");

        public async Task<bool> CreateAsync(FlightDTO dto)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/Flight", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(long id, FlightDTO dto)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/Flight/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/Flight/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
