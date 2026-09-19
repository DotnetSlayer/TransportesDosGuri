using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using TransportesDosGuri.Core.Common;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Auth
{
    public class JwtAuthorizationHandler : DelegatingHandler
    {
        private readonly ITokenStorageService _tokenStorage;
        private readonly IServiceProvider _serviceProvider;

        public JwtAuthorizationHandler(
            ITokenStorageService tokenStorage,
            IServiceProvider serviceProvider)
        {
            _tokenStorage = tokenStorage;
            _serviceProvider = serviceProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var path = request.RequestUri?.AbsolutePath ?? string.Empty;

            var isPublicAccountEndpoint =
                path.EndsWith("/Account/Login", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith("/Account/Register", StringComparison.OrdinalIgnoreCase);

            var (accessToken, _) = await _tokenStorage.GetTokensAsync();

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                using var scope = _serviceProvider.CreateScope();
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

                var newToken = await authService.RefreshTokenAsync();

                if (!string.IsNullOrEmpty(newToken))
                {
                    await _tokenStorage.SetTokensAsync(newToken, string.Empty);

                    request.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", newToken);

                    return await base.SendAsync(request, cancellationToken);
                }
            }

            return response;
        }
    }
}