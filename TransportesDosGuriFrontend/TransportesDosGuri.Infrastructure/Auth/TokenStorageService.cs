using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TransportesDosGuri.Core.Common;

namespace TransportesDosGuri.Infrastructure.Auth
{
    public class TokenStorageService : ITokenStorageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        // Variáveis estáticas removidas para evitar vazamento de memória/Sessão entre usuários
        private string? _runtimeAccessToken;
        private string? _runtimeRefreshToken;

        public TokenStorageService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ValueTask SetTokensAsync(string accessToken, string refreshToken)
        {
            _runtimeAccessToken = accessToken;
            _runtimeRefreshToken = refreshToken;

            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                // Guarda nos itens da requisição atual
                context.Items["access_token"] = accessToken;
                context.Items["refresh_token"] = refreshToken;
            }

            return ValueTask.CompletedTask;
        }

        public async ValueTask<(string? AccessToken, string? RefreshToken)> GetTokensAsync()
        {
            // 1. Tenta recuperar da memória da sessão do circuito Blazor
            if (!string.IsNullOrEmpty(_runtimeAccessToken))
            {
                return (_runtimeAccessToken, _runtimeRefreshToken);
            }

            // 2. Se a memória estiver vazia, busca do HttpContext (Cookie do ASP.NET)
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                var token = await context.GetTokenAsync("access_token") ?? context.Items["access_token"]?.ToString();
                var refresh = await context.GetTokenAsync("refresh_token") ?? context.Items["refresh_token"]?.ToString();

                if (!string.IsNullOrEmpty(token))
                {
                    _runtimeAccessToken = token;
                    _runtimeRefreshToken = refresh;
                    return (token, refresh);
                }
            }

            return (null, null);
        }

        public ValueTask ClearTokensAsync()
        {
            _runtimeAccessToken = null;
            _runtimeRefreshToken = null;
            return ValueTask.CompletedTask;
        }
    }
}