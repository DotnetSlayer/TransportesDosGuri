using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Json;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;

        public ScheduleService(HttpClient http, NavigationManager navigationManager)
        {
            _http = http;
            _navigationManager = navigationManager;
        }

        public async Task<List<ScheduleDTO>> GetAllAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<ScheduleDTO>>("/api/v1/Schedule") ?? new();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized || ex.StatusCode == HttpStatusCode.Forbidden)
            {
                _navigationManager.NavigateTo("/error", replace: true);
                return new List<ScheduleDTO>();
            }
        }

        public async Task<ScheduleDTO?> GetByIdAsync(long id)
        {
            try
            {
                return await _http.GetFromJsonAsync<ScheduleDTO>($"/api/v1/Schedule/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized || ex.StatusCode == HttpStatusCode.Forbidden)
            {
                _navigationManager.NavigateTo("/error", replace: true);
                return null;
            }
        }

        public async Task<bool> CreateAsync(ScheduleDTO dto)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/Schedule", dto);
            return HandleResponse(response);
        }

        public async Task<bool> UpdateAsync(long id, ScheduleDTO dto)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/Schedule/{id}", dto);
            return HandleResponse(response);
        }
        public async Task<(bool Success, string? Error)> DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/Schedule/{id}");

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
                return (false, error?.Message ?? "Não foi possível excluir a escala.");
            }
            catch
            {
                return (false, "Não foi possível excluir a escala.");
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