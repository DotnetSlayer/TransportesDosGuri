using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using TransportesDosGuri.Core.Common;
using TransportesDosGuri.Core.DTOs;
using TransportesDosGuri.Core.Interfaces;
using TransportesDosGuri.Infrastructure.Auth;

namespace TransportesDosGuri.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenStorageService _tokenStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(
            HttpClient httpClient,
            ITokenStorageService tokenStorage,
            AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _tokenStorage = tokenStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task<bool> RegisterAsync(RegisterDTO registerDto)
        {
            var response = await _httpClient.PostAsJsonAsync("", registerDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LoginAsync(LoginDTO loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("", loginDto);

                if (!response.IsSuccessStatusCode)
                    return false;

                var result = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();

                var jwtToken = result?.TokenData?.JwtToken;
                var refreshToken = result?.TokenData?.RefreshToken;

                if (string.IsNullOrEmpty(jwtToken) || string.IsNullOrEmpty(refreshToken))
                {
                    Console.WriteLine("[LOGIN ERROR] Token JWT ou RefreshToken não foram encontrados no payload.");
                    return false;
                }

                await _tokenStorage.SetTokensAsync(jwtToken, refreshToken);
                ((CustomAuthStateProvider)_authStateProvider).MarkUserAsAuthenticated(jwtToken);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LOGIN EXCEPTION] Erro: {ex.Message}");
                return false;
            }
        }

        public async Task<string?> RefreshTokenAsync()
        {
            try
            {
                var (accessToken, refreshToken) = await _tokenStorage.GetTokensAsync();

                if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
                    return null;

                var response = await _httpClient.PostAsJsonAsync("", new RefreshTokenDTO
                {
                    Token = accessToken,
                    RefreshToken = refreshToken
                });

                if (!response.IsSuccessStatusCode)
                {
                    await LogoutAsync();
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();

                var newJwtToken = result?.TokenData?.JwtToken;
                var newRefreshToken = result?.TokenData?.RefreshToken;

                if (string.IsNullOrEmpty(newJwtToken) || string.IsNullOrEmpty(newRefreshToken))
                {
                    await LogoutAsync();
                    return null;
                }

                await _tokenStorage.SetTokensAsync(newJwtToken, newRefreshToken);
                ((CustomAuthStateProvider)_authStateProvider).MarkUserAsAuthenticated(newJwtToken);

                return newJwtToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REFRESH TOKEN EXCEPTION] Erro: {ex.Message}");
                await LogoutAsync();
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            await _tokenStorage.ClearTokensAsync();
            ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOut();
        }
    }
}