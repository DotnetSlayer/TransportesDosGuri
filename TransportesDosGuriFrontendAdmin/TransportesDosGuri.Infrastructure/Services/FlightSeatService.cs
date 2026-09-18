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
    public class FlightSeatService : IFlightSeatService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;

        public FlightSeatService(HttpClient http, NavigationManager navigationManager)
        {
            _http = http;
            _navigationManager = navigationManager;
        }

        public async Task<List<FlightSeatDTO>> GetAllAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<FlightSeatDTO>>("/api/v1/FlightSeat") ?? new();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized || ex.StatusCode == HttpStatusCode.Forbidden)
            {
                _navigationManager.NavigateTo("/error", replace: true);
                return new List<FlightSeatDTO>();
            }
        }

        public async Task<FlightSeatDTO?> GetByIdAsync(long id)
        {
            try
            {
                return await _http.GetFromJsonAsync<FlightSeatDTO>($"/api/v1/FlightSeat/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized || ex.StatusCode == HttpStatusCode.Forbidden)
            {
                _navigationManager.NavigateTo("/error", replace: true);
                return null;
            }
        }

        public async Task<bool> CreateAsync(FlightSeatDTO dto)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/FlightSeat", dto);
            return HandleResponse(response);
        }

        public async Task<bool> UpdateAsync(long id, FlightSeatDTO dto)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/FlightSeat/{id}", dto);
            return HandleResponse(response);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/FlightSeat/{id}");

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
                return (false, error?.Message ?? "Não foi possível excluir o assento de voo.");
            }
            catch
            {
                return (false, "Não foi possível excluir o assento de voo.");
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