using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Components;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class SeatService : ISeatService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;

        public SeatService(HttpClient http, NavigationManager navigationManager)
        {
            _http = http;
            _navigationManager = navigationManager;
        }

        public async Task<List<SeatDTO>> GetAllAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<SeatDTO>>("/api/v1/Seat") ?? new();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized || ex.StatusCode == HttpStatusCode.Forbidden)
            {
                _navigationManager.NavigateTo("/error", replace: true);
                return new List<SeatDTO>();
            }
        }

        public async Task<SeatDTO?> GetByIdAsync(long id)
        {
            try
            {
                return await _http.GetFromJsonAsync<SeatDTO>($"/api/v1/Seat/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized || ex.StatusCode == HttpStatusCode.Forbidden)
            {
                _navigationManager.NavigateTo("/error", replace: true);
                return null;
            }
        }

        public async Task<bool> CreateAsync(SeatDTO dto)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/Seat", dto);
            return HandleResponse(response);
        }

        public async Task<bool> UpdateAsync(long id, SeatDTO dto)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/Seat/{id}", dto);
            return HandleResponse(response);
        }
        public async Task<(bool Success, string? Error)> DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/Seat/{id}");

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
                return (false, error?.Message ?? "Não foi possível excluir o assento.");
            }
            catch
            {
                return (false, "Não foi possível excluir o assento.");
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