using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using TransportesDosGuri.Core.Common;
using TransportesDosGuri.Core.Interfaces;
using TransportesDosGuri.Infrastructure.Auth;
using TransportesDosGuri.Infrastructure.Services;

namespace TransportesDosGuri.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string apiBaseUrl)
    {
        services.AddScoped<ITokenStorageService, TokenStorageService>();

        services.AddAuthorizationCore();
        services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

        services.AddTransient<JwtAuthorizationHandler>();

        services.AddHttpClient("WebAPI", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        })
        .AddHttpMessageHandler<JwtAuthorizationHandler>();

        services.AddScoped(sp =>
            sp.GetRequiredService<IHttpClientFactory>().CreateClient("WebAPI"));

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}