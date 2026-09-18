using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
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
            var response = await _http.GetAsync("api/v1/Trip");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<TripDTO>>()
                       ?? new List<TripDTO>();
            }

            return new List<TripDTO>();
        }

        public async Task<List<TripDTO>> SearchAsync(string? searchTerm)
        {
            var url = "api/v1/trip/search";

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                url += $"?searchTerm={Uri.EscapeDataString(searchTerm)}";
            }

            var response = await _http.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<TripDTO>>()
                       ?? new List<TripDTO>();
            }

            return new List<TripDTO>();
        }

        public async Task<TripDetailsDTO?> GetDetailsAsync(long id)
        {
            var response = await _http.GetAsync($"api/v1/trip/{id}/details");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TripDetailsDTO>();
            }

            return null;
        }
    }
}