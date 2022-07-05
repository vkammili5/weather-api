using Microsoft.EntityFrameworkCore;
using WeatherAPI.Models;
using WeatherAPI.Services;
using WeatherAPI.Services.HttpClients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IHttpClientService, HttpClientService>();

if (builder.Environment.EnvironmentName == "Testing")
{
    builder.Services.AddDbContext<CityContext>(option =>
        option.UseInMemoryDatabase("cityweatherapi"));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("cityweatherapi");
    builder.Services.AddDbContext<CityContext>(option => option.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString)));
}

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();