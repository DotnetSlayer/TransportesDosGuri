using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using TransportesDosGuri.Core.Common;
using TransportesDosGuri.Core.Interfaces;
using TransportesDosGuri.Infrastructure.Auth;
using TransportesDosGuri.Infrastructure.Services;
using TransportesDosGuri.WebUI.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;

    options.MimeTypes = new[]
    {
        "text/plain",
        "text/css",
        "text/html",
        "application/javascript",
        "application/json",
        "application/wasm",
        "image/svg+xml"
    };
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "TransportesDosGuri.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.LoginPath = "/error";
        options.AccessDeniedPath = "/error";
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<ITokenStorageService, TokenStorageService>();
builder.Services.AddTransient<JwtAuthorizationHandler>();



var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "url_webapi_producao";

builder.Services.AddHttpClient<IAuthService, AuthService>(c => c.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddHttpClient<IPurchaseService, PurchaseService>(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<JwtAuthorizationHandler>();
builder.Services.AddHttpClient<ITripService, TripService>(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<JwtAuthorizationHandler>();
builder.Services.AddHttpClient<IUserRequestService, UserRequestService>(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<JwtAuthorizationHandler>();
builder.Services.AddHttpClient<IUserService, UserService>(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<JwtAuthorizationHandler>();

builder.Services.AddHttpClient<ZipCodeApiService>(c =>
{
    c.BaseAddress = new Uri(apiBaseUrl);
    c.Timeout = TimeSpan.FromSeconds(15);
});

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseResponseCompression();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        const int durationInSeconds = 60 * 60 * 24 * 30;

        ctx.Context.Response.Headers.CacheControl =
            $"public,max-age={durationInSeconds},immutable";
    }
});

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
