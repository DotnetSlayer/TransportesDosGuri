
using Asp.Versioning;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;

using TransportesDosGuri.Core.Application.ServiceContracts;
using TransportesDosGuri.Core.Application.ServiceContracts.Asaas;
using TransportesDosGuri.Core.Application.ServiceContracts.Identity;
using TransportesDosGuri.Core.Application.ServiceContracts.Jwt;
using TransportesDosGuri.Core.Application.ServiceContracts.QuestPDF;

using TransportesDosGuri.Core.Application.Services;
using TransportesDosGuri.Core.Application.Services.Identity;
using TransportesDosGuri.Core.Application.Services.Jwt;

using TransportesDosGuri.Core.Domain.Entities.Identity;
using TransportesDosGuri.Core.Domain.RepositoryContracts;
using TransportesDosGuri.Core.Domain.RepositoryContracts.Misc;

using TransportesDosGuri.Infrastructure.Data;
using TransportesDosGuri.Infrastructure.ExternalServices.Asaas;
using TransportesDosGuri.Infrastructure.IdentityContext;
using TransportesDosGuri.Infrastructure.QuestPDF;
using TransportesDosGuri.Infrastructure.Repositories;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

///////////////////////////////////////////////////////////////////////////////////////////////////
// QUESTPDF
///////////////////////////////////////////////////////////////////////////////////////////////////

QuestPDF.Settings.License =
    QuestPDF.Infrastructure.LicenseType.Community;

///////////////////////////////////////////////////////////////////////////////////////////////////
// CONTROLLERS
///////////////////////////////////////////////////////////////////////////////////////////////////

builder.Services.AddControllers(options =>
{
    options.Filters.Add(
        new ProducesAttribute("application/json")
    );

    options.Filters.Add(
        new ConsumesAttribute("application/json")
    );

    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.Filters.Add(
        new AuthorizeFilter(policy)
    );
});

///////////////////////////////////////////////////////////////////////////////////////////////////
// API VERSIONING
///////////////////////////////////////////////////////////////////////////////////////////////////

builder.Services
    .AddApiVersioning(config =>
    {
        config.ApiVersionReader =
            ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new QueryStringApiVersionReader("api-version"),
                new HeaderApiVersionReader("api-version")
            );

        config.DefaultApiVersion =
            new ApiVersion(1, 0);

        config.AssumeDefaultVersionWhenUnspecified =
            true;

        config.ReportApiVersions =
            true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat =
            "'v'VVV";

        options.SubstituteApiVersionInUrl =
            true;
    });

///////////////////////////////////////////////////////////////////////////////////////////////////
// OPEN API
///////////////////////////////////////////////////////////////////////////////////////////////////

builder.Services.AddOpenApi();

///////////////////////////////////////////////////////////////////////////////////////////////////
// DAPPER
///////////////////////////////////////////////////////////////////////////////////////////////////

Dapper.SqlMapper.AddTypeHandler(
    new DateOnlyTypeHandler()
);

///////////////////////////////////////////////////////////////////////////////////////////////////
// APPLICATION SERVICES
///////////////////////////////////////////////////////////////////////////////////////////////////

builder.Services.AddHttpClient();

builder.Services.AddScoped<
    IAircraftService,
    AircraftService
>();

builder.Services.AddScoped<
    IAircraftRepository,
    AircraftRepository
>();

builder.Services.AddScoped<
    IAirportService,
    AirportService
>();

builder.Services.AddScoped<
    IAirportRepository,
    AirportRepository
>();

builder.Services.AddScoped<
    IAsaasIntegrationService,
    AsaasIntegrationService
>();

builder.Services.AddScoped<
    IAsaasIntegrationRepository,
    AsaasIntegrationRepository
>();

builder.Services.AddScoped<
    IFlightSeatService,
    FlightSeatService
>();

builder.Services.AddScoped<
    IFlightSeatRepository,
    FlightSeatRepository
>();

builder.Services.AddScoped<
    IFlightService,
    FlightService
>();

builder.Services.AddScoped<
    IFlightRepository,
    FlightRepository
>();

builder.Services.AddScoped<
    IPurchaseService,
    PurchaseService
>();

builder.Services.AddScoped<
    IPurchaseRepository,
    PurchaseRepository
>();

builder.Services.AddScoped<
    IReservationService,
    ReservationService
>();

builder.Services.AddScoped<
    IReservationRepository,
    ReservationRepository
>();

builder.Services.AddScoped<
    IScheduleService,
    ScheduleService
>();

builder.Services.AddScoped<
    IScheduleRepository,
    ScheduleRepository
>();

builder.Services.AddScoped<
    ISeatService,
    SeatService
>();

builder.Services.AddScoped<
    ISeatRepository,
    SeatRepository
>();

builder.Services.AddScoped<
    ITripService,
    TripService
>();

builder.Services.AddScoped<
    ITripRepository,
    TripRepository
>();

builder.Services.AddScoped<
    IUserRequestService,
    UserRequestService
>();

builder.Services.AddScoped<
    IUserRequestRepository,
    UserRequestRepository
>();

builder.Services.AddScoped<
    IAsaasCheckoutService, AsaasCheckoutService
>();

///////////////////////////////////////////////////////////////////////////////////////////////////
// IDENTITY / JWT / PDF
///////////////////////////////////////////////////////////////////////////////////////////////////

builder.Services.AddScoped<
    IIdentityService,
    IdentityService
>();

builder.Services.AddScoped<
    IDbConnectionFactory,
    DbConnectionFactory
>();

builder.Services.AddScoped<
    IUserRepository,
    UserRepository
>();

builder.Services.AddTransient<
    IJwtService,
    JwtService
>();

builder.Services.AddScoped<
    IReceiptPdfGenerator,
    ReceiptPdfGenerator
>();

///////////////////////////////////////////////////////////////////////////////////////////////////
// ASAAS
///////////////////////////////////////////////////////////////////////////////////////////////////

builder.Services.AddHttpClient<IAsaasGateway, AsaasGateway>(client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "TransportesDosGuri/1.0");
});

builder.Services.AddScoped<AsaasClient>();

///////////////////////////////////////////////////////////////////////////////////////////////////
// SWAGGER
///////////////////////////////////////////////////////////////////////////////////////////////////

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    var xmlPath = Path.Combine(
        AppContext.BaseDirectory,
        "api.xml"
    );

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    options.SwaggerDoc(
        "v1",
        new Microsoft.OpenApi.OpenApiInfo
        {
            Title = "Transportes dos Guri WEB API",
            Version = "1.0"
        }
    );
});

///////////////////////////////////////////////////////////////////////////////////////////////////
// CORS
///////////////////////////////////////////////////////////////////////////////////////////////////

var allowedOrigins =
    builder.Configuration
        .GetSection("AllowedOrigins")
        .Get<string[]>();

if (allowedOrigins == null ||
    allowedOrigins.Length == 0)
{
    throw new InvalidOperationException(
        "Nenhuma origem foi configurada em AllowedOrigins."
    );
}

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    );
});

///////////////////////////////////////////////////////////////////////////////////////////////////
// DATABASE
///////////////////////////////////////////////////////////////////////////////////////////////////

builder.Services.AddDbContext<ApplicationUserDbContext>(
    options =>
    {
        options.UseSqlServer(
            builder.Configuration
                .GetConnectionString(
                    "DefaultConnection"
                )
        );
    }
);

///////////////////////////////////////////////////////////////////////////////////////////////////
// ASP.NET IDENTITY
///////////////////////////////////////////////////////////////////////////////////////////////////

builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>(
        options =>
        {
            options.Password.RequiredLength = 8;

            options.Password.RequireNonAlphanumeric =
                false;

            options.Password.RequireUppercase =
                true;

            options.Password.RequireDigit =
                true;
        }
    )
    .AddUserStore<UserStore<
        ApplicationUser,
        ApplicationRole,
        ApplicationUserDbContext,
        long,
        IdentityUserClaim<long>,
        IdentityUserRole<long>,
        IdentityUserLogin<long>,
        IdentityUserToken<long>,
        IdentityRoleClaim<long>
    >>()
    .AddRoleStore<RoleStore<
        ApplicationRole,
        ApplicationUserDbContext,
        long,
        IdentityUserRole<long>,
        IdentityRoleClaim<long>
    >>()
    .AddDefaultTokenProviders();

///////////////////////////////////////////////////////////////////////////////////////////////////
// JWT CONFIGURATION
///////////////////////////////////////////////////////////////////////////////////////////////////

var jwtKey =
    builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "Jwt:Key não foi configurado."
    );
}

var jwtIssuer =
    builder.Configuration["Jwt:Issuer"];

if (string.IsNullOrWhiteSpace(jwtIssuer))
{
    throw new InvalidOperationException(
        "Jwt:Issuer não foi configurado."
    );
}

var jwtAudience =
    builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "Jwt:Audience não foi configurado."
    );
}

///////////////////////////////////////////////////////////////////////////////////////////////////
// AUTHENTICATION
///////////////////////////////////////////////////////////////////////////////////////////////////

builder.Services
    .AddAuthentication(
        options =>
        {
            options.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;

            options.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
        }
    )
    .AddJwtBearer(
        options =>
        {
            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    // Audience
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,

                    // Issuer
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,

                    // Expiração
                    ValidateLifetime = true,

                    // Assinatura
                    ValidateIssuerSigningKey = true,

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                jwtKey
                            )
                        ),

                    // Claims
                    RoleClaimType =
                        System.Security.Claims.ClaimTypes.Role,

                    NameClaimType =
                        System.Security.Claims.ClaimTypes.Name,

                    // Evita tolerância de tempo entre servidores
                    ClockSkew = TimeSpan.Zero
                };
        }
    );

///////////////////////////////////////////////////////////////////////////////////////////////////
// AUTHORIZATION
///////////////////////////////////////////////////////////////////////////////////////////////////

builder.Services.AddAuthorization(
    options =>
    {
        options.AddPolicy(
            "AdminOnly",
            policy =>
            {
                policy.RequireRole("Admin");
            }
        );
    }
);

///////////////////////////////////////////////////////////////////////////////////////////////////
// BUILD
///////////////////////////////////////////////////////////////////////////////////////////////////

var app = builder.Build();

///////////////////////////////////////////////////////////////////////////////////////////////////
// DEVELOPMENT
///////////////////////////////////////////////////////////////////////////////////////////////////

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();

    app.UseSwaggerUI(
        options =>
        {
            options.SwaggerEndpoint(
                "/swagger/v1/swagger.json",
                "Transportes dos Guri API v1"
            );
        }
    );
}

///////////////////////////////////////////////////////////////////////////////////////////////////
// HTTP PIPELINE
///////////////////////////////////////////////////////////////////////////////////////////////////

app.UseRouting();

///////////////////////////////////////////////////////////////////////////////////////////////////
// CORS
///////////////////////////////////////////////////////////////////////////////////////////////////

app.UseCors("AllowFrontend");

///////////////////////////////////////////////////////////////////////////////////////////////////
// HTTPS
///////////////////////////////////////////////////////////////////////////////////////////////////

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

///////////////////////////////////////////////////////////////////////////////////////////////////
// AUTHENTICATION
///////////////////////////////////////////////////////////////////////////////////////////////////

app.UseAuthentication();

///////////////////////////////////////////////////////////////////////////////////////////////////
// AUTHORIZATION
///////////////////////////////////////////////////////////////////////////////////////////////////

app.UseAuthorization();

///////////////////////////////////////////////////////////////////////////////////////////////////
// CONTROLLERS
///////////////////////////////////////////////////////////////////////////////////////////////////

app.MapControllers();

///////////////////////////////////////////////////////////////////////////////////////////////////
// RUN
///////////////////////////////////////////////////////////////////////////////////////////////////

app.Run();

///////////////////////////////////////////////////////////////////////////////////////////////////
// DAPPER DATEONLY HANDLER
///////////////////////////////////////////////////////////////////////////////////////////////////

public class DateOnlyTypeHandler
    : SqlMapper.TypeHandler<DateOnly>
{
    public override void SetValue(
        IDbDataParameter parameter,
        DateOnly value)
    {
        parameter.Value =
            value.ToDateTime(
                TimeOnly.MinValue
            );
    }

    public override DateOnly Parse(
        object value)
    {
        return DateOnly.FromDateTime(
            (DateTime)value
        );
    }
}
