using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using WeatherAPI.Models;
using WeatherAPI.Services.CityServices;
using WeatherAPI.Services.HttpClients;
using WeatherAPI.Services.WeatherServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IHttpClientService, HttpClientService>();

var connectionString = builder.Configuration.GetConnectionString("cityweatherapi");
builder.Services.AddDbContext<CityContext>(option => option.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
    .AddUrlGroup(uri: new Uri("https://api.open-meteo.com/v1/forecast?latitude=10&longitude=10"),
        name: "Open-meteo Forecast Public API")
    .AddUrlGroup(uri: new Uri("https://geocoding-api.open-meteo.com/v1/search?name=Berlin"),
        name: "Open-meteo Geocoding Public API")
    .AddMySql(connectionString, name: "City Database");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();