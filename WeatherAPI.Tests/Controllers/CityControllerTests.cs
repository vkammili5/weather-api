using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherAPI.Controllers;
using WeatherAPI.Models;
using WeatherAPI.Services;

namespace WeatherAPI.Tests.Controllers;
internal class CityControllerTests
{
    private Mock<ICityService> _cityService;

    CityController _controller;

    [SetUp]
    public void Setup()
    {
        _cityService = new Mock<ICityService>();
        _controller = new CityController(_cityService.Object);
    }

    [Test]
    public async Task GetCityByCityNameAsync_Should_Return_Correct_City()
    {
        // Arrange
        City expectedCity = new City()
        {
            name = "Berlin",
            latitude = 52.52,
            longitude = 13.419998
        };

        _cityService.Setup(c => c.GetCityByCityNameAsync("Berlin"))
            .ReturnsAsync(expectedCity);

        // Act
        var result = await _controller.GetCityByCityNameAsync("Berlin");

        // Assert
        result.Should().BeOfType(typeof(ActionResult<City>));
        result.Value.Should().BeEquivalentTo(expectedCity);
    }

    [Test]
    public async Task GetCityByCityNameAsync_With_Invalid_CityName_Should_Return_BadRequest()
    {
        // Arrange
        _cityService.Setup(c => c.GetCityByCityNameAsync("asjdkflsdj"))
            .Throws<HttpRequestException>();

        // Act
        var result = await _controller.GetCityByCityNameAsync("asjdkflsdj");

        // Assert
        result.Result.Should().BeOfType(typeof(BadRequestObjectResult));
    }

    [Test]
    public async Task AddCityAsync_Creates_A_City()
    {
        // Arrange
        City newCity = new City()
        {
            name = "NewCity",
            latitude = 10,
            longitude = 20
        };

        _cityService.Setup(c => c.AddCityAsync(newCity))
            .ReturnsAsync(newCity);

        // Act
        var result = await _controller.AddCityAsync(newCity);

        // Assert
        result.Should().BeOfType(typeof(ActionResult<City>));
        result.Value.Should().BeEquivalentTo(newCity);
    }

    [Test]
    public async Task AddCityAsync_With_City_Already_Exist_Should_Conflict()
    {
        // Arrange
        City newCity = new City()
        {
            name = "NewCity",
            latitude = 10,
            longitude = 20
        };

        _cityService.Setup(c => c.CityExists(newCity.name))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.AddCityAsync(newCity);

        // Assert
        result.Result.Should().BeOfType(typeof(ConflictObjectResult));
    }
}
