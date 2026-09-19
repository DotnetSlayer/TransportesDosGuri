using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Json;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class FlightService : IFlightService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;

        public FlightService(HttpClient http, NavigationManager navigationManager)
        {
            _http = http;
            _navigationManager = navigationManager;
        }

        public async Task<List<FlightDTO>> GetAllAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<FlightDTO>>("/api/v1/Flight") ?? new();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized || ex.StatusCode == HttpStatusCode.Forbidden)
            {
                _navigationManager.NavigateTo("/error", replace: true);
                return new List<FlightDTO>();
            }
        }

        public async Task<FlightDTO?> GetByIdAsync(long id)
        {
            try
            {
                return await _http.GetFromJsonAsync<FlightDTO>($"/api/v1/Flight/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized || ex.StatusCode == HttpStatusCode.Forbidden)
            {
                _navigationManager.NavigateTo("/error", replace: true);
                return null;
            }
        }

        public async Task<bool> CreateAsync(FlightDTO dto)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/Flight", dto);
            return HandleResponse(response);
        }

        public async Task<bool> UpdateAsync(long id, FlightDTO dto)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/Flight/{id}", dto);
            return HandleResponse(response);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/Flight/{id}");

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
                return (false, error?.Message ?? "Não foi possível excluir o voo.");
            }
            catch
            {
                return (false, "Não foi possível excluir o voo.");
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