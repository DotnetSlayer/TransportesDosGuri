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
            var url = $"api/v1/Trip/{id}/details";

            try
            {
                var response = await _http.GetAsync(url);

                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"GET {url}");
                Console.WriteLine($"Status: {(int)response.StatusCode} - {response.StatusCode}");
                Console.WriteLine($"Response: {content}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content
                        .ReadFromJsonAsync<TripDetailsDTO>();
                }

                throw new HttpRequestException(
                    $"Erro ao buscar detalhes da viagem. " +
                    $"Status: {(int)response.StatusCode} ({response.StatusCode}). " +
                    $"Resposta: {content}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO TripService: {ex}");

                throw;
            }
        }
    }
}