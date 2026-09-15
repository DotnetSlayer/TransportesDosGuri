using TransportesDosGuri.Core.Interfaces;
using TransportesDosGuri.Infrastructure;
using TransportesDosGuri.Web.Services;
using TransportesDosGuri.WebUI.Components;

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte aos componentes do Blazor Server
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Pega a URL da API do appsettings.json ou usa a porta padrão
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "";

// Injeta sua camada Infrastructure de consumo da Web API
builder.Services.AddInfrastructure(apiBaseUrl);

builder.Services.AddHttpClient<IAircraftService, AircraftService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();