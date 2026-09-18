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
    public class PurchaseService : IPurchaseService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;

        public PurchaseService(HttpClient http, NavigationManager navigationManager)
        {
            _http = http;
            _navigationManager = navigationManager;
        }

        public async Task<PurchaseResultDTO?> BuySeatsAsync(BuySeatRequestDTO request)
        {
            var response = await _http.PostAsJsonAsync("api/v1/Purchase/buy", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PurchaseResultDTO>();
            }

            return null;
        }

        public async Task<PurchaseDetailsDTO?> GetMyPurchaseAsync(long purchaseId)
        {
            var response = await _http.GetAsync($"api/v1/Purchase/my/{purchaseId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PurchaseDetailsDTO>();
            }

            return null;
        }

        public async Task<bool> ProcessPaymentAsync(long purchaseId)
        {
            var response = await _http.PostAsync($"api/v1/Purchase/my/{purchaseId}/payment", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<PurchaseDetailsDTO>> GetMyTripsAsync()
        {
            var response = await _http.GetAsync("api/v1/Purchase/my-trips");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<PurchaseDetailsDTO>>()
                       ?? new List<PurchaseDetailsDTO>();
            }

            return new List<PurchaseDetailsDTO>();
        }

        public async Task<byte[]?> DownloadReceiptAsync(long purchaseId)
        {
            var response = await _http.GetAsync($"api/v1/Purchase/my/{purchaseId}/receipt");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }

            return null;
        }
    }
}