using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Models;
using WeatherAPI.Services;

namespace WeatherAPI.Controllers;

[ApiController]
[Route("api/v1/weather")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet("{latitude}/{longitude}")]
    public async Task<ActionResult<Weather>> GetWeatherByLatLon(double latitude, double longitude)
    {
        Weather weather = await _weatherService.GetWeatherByLatLon(latitude, longitude);

        return weather;
    }
}
