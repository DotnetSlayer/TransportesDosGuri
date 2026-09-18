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
    public class UserRequestService : IUserRequestService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;

        public UserRequestService(HttpClient http, NavigationManager navigationManager)
        {
            _http = http;
            _navigationManager = navigationManager;
        }

        public async Task<List<UserRequestDTO>> GetAllAsync()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<UserRequestDTO>>("/api/v1/UserRequest") ?? new();
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized || ex.StatusCode == HttpStatusCode.Forbidden)
            {
                _navigationManager.NavigateTo("/error", replace: true);
                return new List<UserRequestDTO>();
            }
        }

        public async Task<UserRequestDTO?> GetByIdAsync(long id)
        {
            try
            {
                return await _http.GetFromJsonAsync<UserRequestDTO>($"/api/v1/UserRequest/{id}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized || ex.StatusCode == HttpStatusCode.Forbidden)
            {
                _navigationManager.NavigateTo("/error", replace: true);
                return null;
            }
        }

        public async Task<bool> CreateAsync(UserRequestDTO dto)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/UserRequest", dto);
            return HandleResponse(response);
        }

        public async Task<bool> UpdateAsync(long id, UserRequestDTO dto)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/UserRequest/{id}", dto);
            return HandleResponse(response);
        }
        public async Task<(bool Success, string? Error)> DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/UserRequest/{id}");

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
                return (false, error?.Message ?? "Não foi possível excluir a solicitação.");
            }
            catch
            {
                return (false, "Não foi possível excluir a solicitação.");
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