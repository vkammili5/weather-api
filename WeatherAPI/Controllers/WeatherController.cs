using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Models;
using WeatherAPI.Services.CityServices;
using WeatherAPI.Services.WeatherServices;

namespace WeatherAPI.Controllers;

[ApiController]
[Route("api/v1/weather")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;
    private readonly ICityService _cityService;

    public WeatherController(IWeatherService weatherService, ICityService cityService)
    {
        _weatherService = weatherService;
        _cityService = cityService;
    }

    [HttpGet("{latitude}/{longitude}")]
    public async Task<ActionResult<Weather>> GetWeatherByLatLon(double latitude, double longitude)
    {
        try
        {
            Weather weather = await _weatherService.GetWeatherByLatLonAsync(latitude, longitude);
            return weather;
        }
        catch (HttpRequestException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{cityName}")]
    public async Task<ActionResult<Weather>> GetWeatherByCityName(string cityName)
    {
        try
        {
            City city = await _cityService.GetCityByCityNameAsync(cityName);
            Weather weather = await _weatherService.GetWeatherByLatLonAsync(city.Latitude, city.Longitude);
            return weather; 
        }
        catch (HttpRequestException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
