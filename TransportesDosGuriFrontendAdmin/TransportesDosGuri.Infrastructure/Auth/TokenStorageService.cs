using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using TransportesDosGuri.Core.Common;

namespace TransportesDosGuri.Infrastructure.Auth
{
    public class TokenStorageService : ITokenStorageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static string? _cachedAccessToken;
        private static string? _cachedRefreshToken;

        public TokenStorageService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ValueTask SetTokensAsync(string accessToken, string refreshToken)
        {
            _cachedAccessToken = accessToken;
            _cachedRefreshToken = refreshToken;
            return ValueTask.CompletedTask;
        }

        public async ValueTask<(string? AccessToken, string? RefreshToken)> GetTokensAsync()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                var token = await context.GetTokenAsync("access_token");
                var refresh = await context.GetTokenAsync("refresh_token");
                if (!string.IsNullOrEmpty(token))
                    return (token, refresh);
            }

            return (_cachedAccessToken, _cachedRefreshToken);
        }

        public ValueTask ClearTokensAsync()
        {
            _cachedAccessToken = null;
            _cachedRefreshToken = null;
            return ValueTask.CompletedTask;
        }
    }
}