using AspNetCoreRateLimit;
using CartonCaps.Api.BL;
using CartonCaps.Api.Domain.Configurations;
using CartonCaps.Api.Domain.Interfaces.BL;
using CartonCaps.Api.Domain.Repositories;
using CartonCaps.Api.Infraestructure;
using CartonCaps.Api.Infraestructure.Repositories;
using CartonCaps.Api.BackgroundServices;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FrontendConfiguration>(
    builder.Configuration.GetSection("FrontendSettings"));
builder.Services.Configure<ReferralLimitsConfiguration>(
    builder.Configuration.GetSection("ReferralLimits"));

builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.EnableEndpointRateLimiting = true;
    options.StackBlockedRequests = false;
    options.HttpStatusCode = 429;
    options.RealIpHeader = "X-Real-IP";
    options.ClientIdHeader = "X-ClientId";
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "POST:/api/referral/invitation-link",
            Period = "1h",
            Limit = 10
        },
        new RateLimitRule
        {
            Endpoint = "POST:/api/referral/referral-registration",
            Period = "1h",
            Limit = 5
        }
    };
});
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddDbContext<CartonCapsContext>(options => options.UseInMemoryDatabase("CartonCapsDb"));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IReferralRepository, ReferralRepository>();
builder.Services.AddScoped<IReferralService, ReferralService>();
builder.Services.AddHostedService<ReferralExpirationService>();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CartonCapsContext>();
    DataSeeder.SeedData(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseIpRateLimiting();

app.UseAuthorization();

app.MapControllers();

app.Run();
