﻿using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("{cityName}", Name = "Get")]
    public async Task<ActionResult<City>> GetCityByCityName(string cityName)
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
    public async Task<ActionResult<City>> AddCity(City newCity)
    {
        if (await _cityService.CityExists(newCity.Name))
            return Conflict(new { message = $"City with city name {newCity.Name} already exists." });

        await _cityService.AddCityAsync(newCity);

        return CreatedAtAction(
            nameof(GetCityByCityName),
            new { cityName = newCity.Name },
            newCity);
    }

    [HttpPut("{cityName}")]
    public async Task<ActionResult<City>> UpdateCity(string cityName, City city)
    {
        if (cityName != city.Name)
            return BadRequest(new { message = $"CityName {cityName} should match city.name {city.Name}" });

        if (!await _cityService.CityExists(cityName))
            return NotFound(new { message = $"CityName {cityName} not found in collection" });

        city = await _cityService.UpdateCityAsync(city);
        return city;
    }

    [HttpDelete("{cityName}")]
    public async Task<IActionResult> DeleteCity(string cityName)
    {
        if (!await _cityService.CityExists(cityName))
            return NotFound(new { message = $"CityName {cityName} not found in collection" });

        await _cityService.DeleteCityAsync(cityName);

        return NoContent();
    }
}
