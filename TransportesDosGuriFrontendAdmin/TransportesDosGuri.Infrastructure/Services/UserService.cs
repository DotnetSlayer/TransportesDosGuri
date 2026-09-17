using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<UserDTO>?> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<UserDTO>>($"/api/v1/Account/GetAll");
        }

        public async Task<UserDTO?> GetByIdAsync(long id)
        {
            var response = await _httpClient.GetAsync($"/api/v1/Account/{id}");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<UserDTO>();
        }

        public async Task<bool> CreateAsync(RegisterDTO registerDto)
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/v1/Account/Register", registerDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(long id, UpdateUserDTO updateDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/v1/Account/{id}", updateDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"/api/v1/Account/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}