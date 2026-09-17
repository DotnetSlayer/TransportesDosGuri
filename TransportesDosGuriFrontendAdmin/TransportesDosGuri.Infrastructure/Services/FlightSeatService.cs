using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class FlightSeatService : IFlightSeatService
    {
        private readonly HttpClient _http;

        public FlightSeatService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<FlightSeatDTO>> GetAllAsync() =>
            await _http.GetFromJsonAsync<List<FlightSeatDTO>>("/api/v1/FlightSeat") ?? new();

        public async Task<FlightSeatDTO?> GetByIdAsync(long id) =>
            await _http.GetFromJsonAsync<FlightSeatDTO>($"/api/v1/FlightSeat/{id}");

        public async Task<bool> CreateAsync(FlightSeatDTO dto)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/FlightSeat", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(long id, FlightSeatDTO dto)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/FlightSeat/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/FlightSeat/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
