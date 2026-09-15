using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class SeatService : ISeatService
    {
        private readonly HttpClient _http;

        public SeatService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<SeatDTO>> GetAllAsync() =>
            await _http.GetFromJsonAsync<List<SeatDTO>>("/api/v1/Seat") ?? new();

        public async Task<SeatDTO?> GetByIdAsync(long id) =>
            await _http.GetFromJsonAsync<SeatDTO>($"/api/v1/Seat/{id}");

        public async Task<bool> CreateAsync(SeatDTO dto)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/Seat", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(long id, SeatDTO dto)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/Seat/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/Seat/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
