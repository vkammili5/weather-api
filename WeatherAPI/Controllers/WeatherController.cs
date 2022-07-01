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
    public async Task<ActionResult<Weather>> GetWeatherByLatLonAsync(double latitude, double longitude)
    {
        try
        {
            Weather weather = await _weatherService.GetWeatherByLatLonAsync(latitude, longitude);
            return weather;
        }
        catch (HttpRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
