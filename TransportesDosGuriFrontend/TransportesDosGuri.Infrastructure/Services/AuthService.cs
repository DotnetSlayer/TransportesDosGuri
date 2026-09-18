using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Security.Claims;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            HttpClient httpClient,
            ITokenStorageService tokenStorage,
            AuthenticationStateProvider authStateProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _tokenStorage = tokenStorage;
            _authStateProvider = authStateProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> RegisterAsync(RegisterDTO registerDto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/v1/Account/Register", registerDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> LoginAsync(LoginDTO loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/v1/Account/Login", loginDto);
                if (!response.IsSuccessStatusCode) return false;

                var result = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();
                var jwtToken = result?.TokenData?.JwtToken;
                var refreshToken = result?.TokenData?.RefreshToken;

                if (string.IsNullOrEmpty(jwtToken) || string.IsNullOrEmpty(refreshToken))
                    return false;

                await _tokenStorage.SetTokensAsync(jwtToken, refreshToken);

                var context = _httpContextAccessor.HttpContext;
                if (context != null)
                {
                    var claims = CustomAuthStateProvider.ParseClaimsFromJwt(jwtToken);
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties();
                    authProperties.StoreTokens(new[]
                    {
                        new AuthenticationToken { Name = "access_token", Value = jwtToken },
                        new AuthenticationToken { Name = "refresh_token", Value = refreshToken }
                    });

                    await context.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity),
                        authProperties);
                }

                ((CustomAuthStateProvider)_authStateProvider).MarkUserAsAuthenticated(jwtToken);
                return true;
            }
            catch
            {
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

                var response = await _httpClient.PostAsJsonAsync("/api/v1/Account/generate-new-jwt-token", new RefreshTokenDTO
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
            catch
            {
                await LogoutAsync();
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                var (accessToken, refreshToken) = await _tokenStorage.GetTokensAsync();

                if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
                {
                    var dto = new RefreshTokenDTO
                    {
                        Token = accessToken,
                        RefreshToken = refreshToken
                    };

                    await _httpClient.PostAsJsonAsync("/api/v1/Account/Logout", dto);
                }
            }
            catch
            {
            }
            finally
            {
                await _tokenStorage.ClearTokensAsync();

                ((CustomAuthStateProvider)_authStateProvider).MarkUserAsLoggedOut();
            }
        }
    }
}
