using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TransportesDosGuri.Core.Application.ServiceContracts;
using TransportesDosGuri.Core.Application.ServiceContracts.Identity;
using TransportesDosGuri.Core.Application.ServiceContracts.Jwt;
using TransportesDosGuri.Core.Application.Services;
using TransportesDosGuri.Core.Application.Services.Identity;
using TransportesDosGuri.Core.Application.Services.Jwt;
using TransportesDosGuri.Core.Domain.Entities.Identity;
using TransportesDosGuri.Core.Domain.RepositoryContracts;
using TransportesDosGuri.Core.Domain.RepositoryContracts.Misc;
using TransportesDosGuri.Infrastructure.Data;
using TransportesDosGuri.Infrastructure.ExternalServices.Asaas;
using TransportesDosGuri.Infrastructure.IdentityContext;
using TransportesDosGuri.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
    options.Filters.Add(new ConsumesAttribute("application/json"));

    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddApiVersioning(config =>
{
    config.ApiVersionReader = new UrlSegmentApiVersionReader();
    config.ApiVersionReader = new QueryStringApiVersionReader();
    config.ApiVersionReader = new HeaderApiVersionReader("api-version");

    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddOpenApi();

///////////////////////////////////////////////////////////////////////////////////////////////////

builder.Services.AddScoped<IAircraftService, AircraftService>();
builder.Services.AddScoped<IAircraftRepository, AircraftRepository>();

builder.Services.AddScoped<IAirportService, AirportService>();
builder.Services.AddScoped<IAirportRepository, AirportRepository>();

builder.Services.AddScoped<IAsaasIntegrationService, AsaasIntegrationService>();
builder.Services.AddScoped<IAsaasIntegrationRepository, AsaasIntegrationRepository>();

builder.Services.AddScoped<IFlightSeatService, FlightSeatService>();
builder.Services.AddScoped<IFlightSeatRepository, FlightSeatRepository>();

builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<IFlightRepository, FlightRepository>();

builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();

builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();

builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();

builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<ITripRepository, TripRepository>();

builder.Services.AddScoped<IUserRequestService, UserRequestService>();
builder.Services.AddScoped<IUserRequestRepository, UserRequestRepository>();

///////////////////////////////////////////////////////////////////////////////////////////////////

builder.Services.AddScoped<IIdentityService, IdentityService>();

builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

builder.Services.AddScoped<IAsaasIntegrationRepository, AsaasIntegrationRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddTransient<IJwtService, JwtService>();

///////////////////////////////////////////////////////////////////////////////////////////////////

builder.Services.AddHttpClient("AsaasClient", client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "TransportesDosGuri/1.0");
});

builder.Services.AddScoped<AsaasClient>();

builder.Services.AddScoped<IAsaasGateway, AsaasGateway>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "api.xml"));

    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo()
    {
        Title = "Transportes dos Guri WEB API",
        Version = "1.0"
    });

});

builder.Services.AddApiVersioning().AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>())
        .WithHeaders("Authorization", "origin", "accept", "content-type")
        .AllowAnyMethod();
    });
});

///////////////////////////////////////////////////////////////////////////////////////////////////

builder.Services.AddDbContext<ApplicationUserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddOpenApi();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;
})
.AddUserStore<UserStore<
    ApplicationUser,
    ApplicationRole,
    ApplicationUserDbContext,
    long,
    IdentityUserClaim<long>,
    IdentityUserRole<long>,
    IdentityUserLogin<long>,
    IdentityUserToken<long>,
    IdentityRoleClaim<long>>>()
.AddRoleStore<RoleStore<
    ApplicationRole,
    ApplicationUserDbContext,
    long,
    IdentityUserRole<long>,
    IdentityRoleClaim<long>>>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateAudience = true,

            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateIssuer = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),


            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "1.0");
});

app.UseRouting();

app.UseCors();

app.UseHsts();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
