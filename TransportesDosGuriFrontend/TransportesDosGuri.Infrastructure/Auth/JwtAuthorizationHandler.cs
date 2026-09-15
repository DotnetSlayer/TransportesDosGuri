using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using TransportesDosGuri.Core.Common;
using TransportesDosGuri.Core.Interfaces;

namespace TransportesDosGuri.Infrastructure.Auth;

public class JwtAuthorizationHandler : DelegatingHandler
{
    private readonly ITokenStorageService _tokenStorage;
    private readonly IServiceProvider _serviceProvider;

    public JwtAuthorizationHandler(ITokenStorageService tokenStorage, IServiceProvider serviceProvider)
    {
        _tokenStorage = tokenStorage;
        _serviceProvider = serviceProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var (accessToken, _) = await _tokenStorage.GetTokensAsync();

        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var authService = _serviceProvider.GetRequiredService<IAuthService>();
            var newToken = await authService.RefreshTokenAsync();

            if (!string.IsNullOrEmpty(newToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                response = await base.SendAsync(request, cancellationToken);
            }
        }

        return response;
    }
}