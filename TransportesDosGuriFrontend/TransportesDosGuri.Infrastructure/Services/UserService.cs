using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;

        public UserService(HttpClient http, NavigationManager navigationManager)
        {
            _http = http;
            _navigationManager = navigationManager;
        }

        public async Task<UserDTO?> GetMeAsync()
        {
            var response = await _http.GetAsync("api/v1/account/me");

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"API retornou {(int)response.StatusCode} ({response.StatusCode}). " +
                    $"Resposta: {content}");
            }

            return await response.Content.ReadFromJsonAsync<UserDTO>();
        }

        public async Task<bool> UpdateMeAsync(UpdateUserDTO updateDto)
        {
            var response = await _http.PutAsJsonAsync("api/v1/account/me", updateDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteMeAsync()
        {
            var response = await _http.DeleteAsync("api/v1/account/me");
            return response.IsSuccessStatusCode;
        }
    }
}