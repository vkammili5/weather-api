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
}
