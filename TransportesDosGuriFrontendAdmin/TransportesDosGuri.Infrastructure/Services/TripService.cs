using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Json;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class TripService : ITripService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;

        public TripService(HttpClient http, NavigationManager navigationManager)
        {
            _http = http;
            _navigationManager = navigationManager;
        }

        public async Task<List<TripDTO>> GetAllAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<TripDTO>>("/api/v1/Trip") ?? new();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized || ex.StatusCode == HttpStatusCode.Forbidden)
            {
                _navigationManager.NavigateTo("/error", replace: true);
                return new List<TripDTO>();
            }
        }

        public async Task<TripDTO?> GetByIdAsync(long id)
        {
            try
            {
                return await _http.GetFromJsonAsync<TripDTO>($"/api/v1/Trip/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized || ex.StatusCode == HttpStatusCode.Forbidden)
            {
                _navigationManager.NavigateTo("/error", replace: true);
                return null;
            }
        }

        public async Task<bool> CreateAsync(TripDTO dto)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/Trip", dto);
            return HandleResponse(response);
        }

        public async Task<bool> UpdateAsync(long id, TripDTO dto)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/Trip/{id}", dto);
            return HandleResponse(response);
        }
        public async Task<(bool Success, string? Error)> DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/Trip/{id}");

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
                return (false, error?.Message ?? "Não foi possível excluir a viagem.");
            }
            catch
            {
                return (false, "Não foi possível excluir a viagem.");
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