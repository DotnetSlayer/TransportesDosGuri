using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly HttpClient _http;

        public ScheduleService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<ScheduleDTO>> GetAllAsync() =>
            await _http.GetFromJsonAsync<List<ScheduleDTO>>("/api/v1/Schedule") ?? new();

        public async Task<ScheduleDTO?> GetByIdAsync(long id) =>
            await _http.GetFromJsonAsync<ScheduleDTO>($"/api/v1/Schedule/{id}");

        public async Task<bool> CreateAsync(ScheduleDTO dto)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/Schedule", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(long id, ScheduleDTO dto)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/Schedule/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/Schedule/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
