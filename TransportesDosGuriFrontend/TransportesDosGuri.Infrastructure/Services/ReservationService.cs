using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class ReservationService : IReservationService
    {
        private readonly HttpClient _http;

        public ReservationService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<ReservationDTO>> GetAllAsync() =>
            await _http.GetFromJsonAsync<List<ReservationDTO>>("/api/v1/Reservation") ?? new();

        public async Task<ReservationDTO?> GetByIdAsync(long id) =>
            await _http.GetFromJsonAsync<ReservationDTO>($"/api/v1/Reservation/{id}");

        public async Task<bool> CreateAsync(ReservationDTO dto)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/Reservation", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(long id, ReservationDTO dto)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/Reservation/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/Reservation/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
