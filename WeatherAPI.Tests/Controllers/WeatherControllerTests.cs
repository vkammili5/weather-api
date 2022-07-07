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
internal class WeatherControllerTests
{
    private WeatherController _controller;
    private Mock<IWeatherService> _mockWeatherService;
    private Mock<ICityService> _mockCityService;

    [SetUp]
    public void Setup()
    {
        // Arrange
        _mockWeatherService = new Mock<IWeatherService>();
        _mockCityService = new Mock<ICityService>();
        _controller = new WeatherController(_mockWeatherService.Object, _mockCityService.Object);
    }

    [Test]
    public async Task GetWeatherByLatLon_Should_Return_Correct_Weather()
    {
        // Arrange
        Weather expectedWeather = new Weather() {
            latitude = 51.5,
            longitude = -0.12,
            weatherCode = WeatherCode.ClearSky,
            whatToPrepare = "wear summer clothings, wear a cap, apply sunscreen"
        };

        _mockWeatherService.Setup(w => w.GetWeatherByLatLonAsync(51.5002, -0.1262))
            .ReturnsAsync(expectedWeather);

        // Act
        var result = await _controller.GetWeatherByLatLonAsync(51.5002, -0.1262);

        // Assert
        result.Should().BeOfType(typeof(ActionResult<Weather>));
        result.Value.Should().BeEquivalentTo(expectedWeather);
    }

    [Test]
    public async Task GetWeatherByLatLon_With_Invalid_LatLon_Should_Return_BadRequest()
    {
        // Arrange
        _mockWeatherService.Setup(w => w.GetWeatherByLatLonAsync(510.5002, -0.1262))
            .Throws<HttpRequestException>();

        // Act
        var result = await _controller.GetWeatherByLatLonAsync(510.5002, -0.1262);

        // Assert
        result.Result.Should().BeOfType(typeof(BadRequestObjectResult));
    }
    [Test]
    public async Task GetWeatherByCityName_Should_Return_Correct_Weather()
    {
        // Arrange
        Weather expectedWeather = new Weather()
        {
            latitude = 52.52,
            longitude = 13.419998,
            weatherCode = WeatherCode.ClearSky,
            whatToPrepare = "wear summer clothings, wear a cap, apply sunscreen"
        };
        City expectedCity = new City()
        {
            name = "Berlin",
            latitude = 52.52,
            longitude = 13.419998
        };

        _mockCityService.Setup(w => w.GetCityByCityNameAsync("Berlin"))
             .ReturnsAsync(expectedCity);

        _mockWeatherService.Setup(w => w.GetWeatherByLatLonAsync(52.52, 13.419998))
            .ReturnsAsync(expectedWeather);

        //act
        var result = await _controller.GetWeatherByCityNameAsync("Berlin");

        // Assert
        result.Should().BeOfType(typeof(ActionResult<Weather>));
        result.Value.Should().BeEquivalentTo(expectedWeather);
    }
    [Test]
    public async Task GetWeatherByCity_Should_Return_BadRequest()
    {
        //arrange
        _mockCityService.Setup(x => x.GetCityByCityNameAsync("dfgsh")).
            Throws<HttpRequestException>();

        //act
        var result = await _controller.GetWeatherByCityNameAsync("dfgsh");

        //Assert
        result.Result.Should().BeOfType(typeof(BadRequestObjectResult));
    }
}
