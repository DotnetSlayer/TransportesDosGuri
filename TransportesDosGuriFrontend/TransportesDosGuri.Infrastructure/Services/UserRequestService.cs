using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class UserRequestService : IUserRequestService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;

        public UserRequestService(HttpClient http, NavigationManager navigationManager)
        {
            _http = http;
            _navigationManager = navigationManager;
        }

        public async Task<UserRequestDTO?> CreateWithPaymentAsync(UserRequestDTO request)
        {
            var response = await _http.PostAsJsonAsync("api/v1/UserRequest/with-payment", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserRequestDTO>();
            }

            return null;
        }
    }
}