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
    public class UserService : IUserService
    {
        private readonly HttpClient _http;
        private readonly NavigationManager _navigationManager;

        public UserService(HttpClient http, NavigationManager navigationManager)
        {
            _http = http;
            _navigationManager = navigationManager;
        }

        public async Task<List<UserDTO>?> GetAllAsync()
        {
            var response = await _http.GetAsync("/api/v1/account");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<UserDTO>>();
            }

            return null;
        }

        public async Task<UserDTO?> GetByIdAsync(long id)
        {
            var response = await _http.GetAsync($"/api/v1/account/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDTO>();
            }

            return null;
        }

        public async Task<bool> CreateAsync(RegisterDTO registerDto)
        {
            var response = await _http.PostAsJsonAsync("/api/v1/account/register", registerDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(long id, UpdateUserDTO updateDto)
        {
            var response = await _http.PutAsJsonAsync($"/api/v1/account/{id}", updateDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var response = await _http.DeleteAsync($"/api/v1/account/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}