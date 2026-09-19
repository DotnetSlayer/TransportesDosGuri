using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Json;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;

        public UserService(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
        }

        public async Task<List<UserDTO>?> GetAllAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<UserDTO>>("/api/v1/Account/GetAll");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized || ex.StatusCode == HttpStatusCode.Forbidden)
            {
                _navigationManager.NavigateTo("/error", replace: true);
                return new List<UserDTO>();
            }
        }

        public async Task<UserDTO?> GetByIdAsync(long id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/v1/Account/{id}");

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<UserDTO>();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized || ex.StatusCode == HttpStatusCode.Forbidden)
            {
                _navigationManager.NavigateTo("/error", replace: true);
                return null;
            }
        }

        public async Task<bool> CreateAsync(RegisterDTO registerDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/v1/Account/Register", registerDto);
            return HandleResponse(response);
        }

        public async Task<bool> UpdateAsync(long id, UpdateUserDTO updateDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/v1/Account/{id}", updateDto);
            return HandleResponse(response);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(long id)
        {
            var response = await _httpClient.DeleteAsync($"/api/v1/Account/{id}");

            if (response.StatusCode == HttpStatusCode.Unauthorized ||
                response.StatusCode == HttpStatusCode.Forbidden)
            {
                _navigationManager.NavigateTo("/error", replace: true);
                return (false, "Sessão expirada. Faça login novamente.");
            }

            if (response.IsSuccessStatusCode)
                return (true, null);

            try
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                return (false, error?.Message ?? "Não foi possível excluir o usuário.");
            }
            catch
            {
                return (false, "Não foi possível excluir o usuário.");
            }
        }

        private bool HandleResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
            {
                _navigationManager.NavigateTo("/error", replace: true);
                return false;
            }

            return response.IsSuccessStatusCode;
        }

        private class ErrorResponse
        {
            public string? Message { get; set; }
        }
    }
}