using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using TransportesDosGuri.Core.Common;
using TransportesDosGuri.Core.Interfaces;
using TransportesDosGuri.Infrastructure;
using TransportesDosGuri.Infrastructure.Auth;
using TransportesDosGuri.Infrastructure.Services;
using TransportesDosGuri.Web.Services;
using TransportesDosGuri.WebUI.Components;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

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

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<ITokenStorageService, TokenStorageService>();
builder.Services.AddTransient<JwtAuthorizationHandler>();

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7030";

builder.Services.AddHttpClient<IAuthService, AuthService>(c => c.BaseAddress = new Uri(apiBaseUrl));

builder.Services.AddHttpClient<IAircraftService, AircraftService>(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<JwtAuthorizationHandler>();
builder.Services.AddHttpClient<IAirportService, AirportService>(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<JwtAuthorizationHandler>();
builder.Services.AddHttpClient<IAsaasIntegrationService, AsaasIntegrationService>(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<JwtAuthorizationHandler>();
builder.Services.AddHttpClient<IAuthService, AuthService>(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<JwtAuthorizationHandler>();
builder.Services.AddHttpClient<IFlightSeatService, FlightSeatService>(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<JwtAuthorizationHandler>();
builder.Services.AddHttpClient<IFlightService, FlightService>(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<JwtAuthorizationHandler>();
builder.Services.AddHttpClient<IPurchaseService, PurchaseService>(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<JwtAuthorizationHandler>();
builder.Services.AddHttpClient<IReservationService, ReservationService>(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<JwtAuthorizationHandler>();
builder.Services.AddHttpClient<IScheduleService, ScheduleService>(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<JwtAuthorizationHandler>();
builder.Services.AddHttpClient<ISeatService, SeatService>(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<JwtAuthorizationHandler>();
builder.Services.AddHttpClient<ITripService, TripService>(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<JwtAuthorizationHandler>();
builder.Services.AddHttpClient<IUserRequestService, UserRequestService>(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<JwtAuthorizationHandler>();
builder.Services.AddHttpClient<IUserService, UserService>(c => c.BaseAddress = new Uri(apiBaseUrl)).AddHttpMessageHandler<JwtAuthorizationHandler>();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();