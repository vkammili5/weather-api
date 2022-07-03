using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Models;
using WeatherAPI.Services;

namespace WeatherAPI.Controllers;

[ApiController]
[Route("api/v1/city")]
public class CityController : ControllerBase
{
    private readonly ICityService _cityService;
    public CityController(ICityService cityService)
    {
        _cityService = cityService;
    }

    [HttpGet("{cityName}")]
    public async Task<ActionResult<City>> GetCityByCityNameAsync(string cityName)
    {
        try
        {
            City city = await _cityService.GetCityByCityNameAsync(cityName);
            return city;
        }
        catch (HttpRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public async Task<ActionResult<City>> AddCityAsync(City newCity)
    {
        if (await _cityService.CityExists(newCity.name))
            return Conflict(new { message = $"City with city name {newCity.name} already exists." });

        return await _cityService.AddCityAsync(newCity);
    }
}
