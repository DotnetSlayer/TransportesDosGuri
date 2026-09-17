using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class TripService : ITripService
    {
        private readonly HttpClient _http;

        public TripService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<TripDTO>> GetAllAsync() =>
            await _http.GetFromJsonAsync<List<TripDTO>>("/api/v1/Trip") ?? new();

        public async Task<TripDTO?> GetByIdAsync(long id) =>
            await _http.GetFromJsonAsync<TripDTO>($"/api/v1/Trip/{id}");

        public async Task<bool> CreateAsync(TripDTO dto)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/Trip", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(long id, TripDTO dto)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/Trip", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/Trip");
            return response.IsSuccessStatusCode;
        }
    }
}
