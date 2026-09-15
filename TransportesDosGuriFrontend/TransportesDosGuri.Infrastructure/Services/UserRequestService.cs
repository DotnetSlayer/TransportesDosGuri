using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class UserRequestService : IUserRequestService
    {
        private readonly HttpClient _http;

        public UserRequestService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<UserRequestDTO>> GetAllAsync() =>
            await _http.GetFromJsonAsync<List<UserRequestDTO>>("/api/v1/UserRequest") ?? new();

        public async Task<UserRequestDTO?> GetByIdAsync(long id) =>
            await _http.GetFromJsonAsync<UserRequestDTO>($"/api/v1/UserRequest/{id}");

        public async Task<bool> CreateAsync(UserRequestDTO dto)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/UserRequest", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(long id, UserRequestDTO dto)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/UserRequest/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/UserRequest/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
