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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<City>>> GetCityList()
    {
        return await _cityService.GetAllCity();
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

    [HttpPost]
    public async Task<ActionResult<City>> AddCityAsync(City newCity)
    {
        if (await _cityService.CityExists(newCity.Name))
            return Conflict(new { message = $"City with city name {newCity.Name} already exists." });

        return await _cityService.AddCityAsync(newCity);
    }

    [HttpPut("{cityName}")]
    public async Task<ActionResult<City>> UpdateCityAsync(string cityName, City city)
    {
        if (cityName != city.Name)
            return BadRequest(new { message = $"CityName {cityName} should match city.name {city.Name}" });

        if (!await _cityService.CityExists(city.Name))
            return NotFound(cityName);

        return await _cityService.UpdateCityAsync(city);
    }

    [HttpDelete("{cityName}")]
    public async Task<IActionResult> DeleteCityAsync(string cityName)
    {
        if (!await _cityService.CityExists(cityName))
            return NotFound(cityName);

        await _cityService.DeleteCityAsync(cityName);

        return NoContent();
    }
}
