
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;
using System.Text.Json.Serialization;
using Taxi.Api.Middleware;
using Taxi.App.Services.AdminService;
using Taxi.App.Services.AuthService;
using Taxi.App.Services.ComplaintService;
using Taxi.App.Services.DriverMatchingService;
using Taxi.App.Services.DriverService.DriverApplicationService;
using Taxi.App.Services.DriverService.DriverStatusService;
using Taxi.App.Services.LocationService.DriverLocationService;
using Taxi.App.Services.LocationService.RideLocationService;
using Taxi.App.Services.PaymentService;
using Taxi.App.Services.PricingService.PriceCalculationService;
using Taxi.App.Services.PricingService.PricingSettingsService;
using Taxi.App.Services.PricingService.RouteService;
using Taxi.App.Services.PricingService.WeatherService;
using Taxi.App.Services.RatingService;
using Taxi.App.Services.RideService;
using Taxi.App.Services.VehicleService.VehicleApplicationService;
using Taxi.App.Services.VehicleService.VehicleManageService;
using Taxi.App.Validators.DriverValidators;
using Taxi.App.Validators.UserValidators;
using Taxi.App.Validators.VehicleValidators;
using Taxi.Infrastructure;
using Taxi.Infrastructure.Options;
using Taxi.Infrastructure.Repos;
using Taxi.Infrastructure.Repos.CancellationRepos;
using Taxi.Infrastructure.Repos.RideMatchingRepos;
using Taxi.Infrastructure.Repos.RideMatchingRepository;
using Taxi.Infrastructure.Services.PasswordHasher;
using Taxi.Infrastructure.Services.TokenGenerator;
using Taxi.Infrastructure.Services.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
  
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection(nameof(AuthOptions)));

var authOptions = builder.Configuration.GetSection(nameof(AuthOptions)).Get<AuthOptions>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("Redis");
    return ConnectionMultiplexer.Connect(connectionString);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = authOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = authOptions.Audience,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authOptions.Key)),
                        ValidateIssuerSigningKey = true,
                    };
                });

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IStringHasher, StringHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
builder.Services.AddScoped<IDriverApplicationService, DriverApplicationService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IVehicleApplicationService, VehicleApplicationService>();
builder.Services.AddScoped<IVehicleManageService, VehicleManageService>();
builder.Services.AddScoped<IDriverStatusService, DriverStatusService>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IPricingSettingsService, PricingSettingsService>();
builder.Services.AddScoped<IPriceCalculationService, PriceCalculationService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IRideService, RideService>();
builder.Services.AddScoped<IDriverLocationService, DriverLocationService>();
builder.Services.AddScoped<IDriverMatchingService, DriverMatchingService>();
builder.Services.AddScoped<IDriverMatchingRepos, DriverMatchingRepos>();
builder.Services.AddScoped<IRideLocationService, RideLocationService>();
builder.Services.AddScoped<ICancellationRepos, CancellationRepos>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IComplaintService, ComplaintsService>();
builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddHostedService<DMBackgroundService>();
builder.Services.AddSignalR();

//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
//builder.Logging.AddDebug();

// Optional: Set a global minimum log level in code
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddValidatorsFromAssemblyContaining<RegistrationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DriverApplicationValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<VehicleValidator>();


string connectionString = builder.Configuration.GetConnectionString("TaxiConStr");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Taxi", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{ }
                        //or
                        //new List<string>()

                    }
                });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ApiExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
