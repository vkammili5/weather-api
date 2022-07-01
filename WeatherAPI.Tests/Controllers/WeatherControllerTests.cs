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

    [SetUp]
    public void Setup()
    {
        // Arrange
        _mockWeatherService = new Mock<IWeatherService>();
        _controller = new WeatherController(_mockWeatherService.Object);
    }

    [Test]
    public async Task GetWeatherByLatLon_Should_Return_Correct_Weather()
    {
        // Arrange
        Weather expectedWeather = new Weather() {
            latitude = 51.5,
            longitude = -0.12,
            startDate = DateTime.Today,
            endDate = DateTime.Today.AddDays(7),
            weatherCodes = new List<WeatherCode>() {
                WeatherCode.ClearSky,
                WeatherCode.ClearSky,
                WeatherCode.ClearSky,
                WeatherCode.Fog,
                WeatherCode.Fog,
                WeatherCode.Fog,
                WeatherCode.RainOrWorse,
            }
        };

        _mockWeatherService.Setup(w => w.GetWeatherByLatLonAsync(51.5002, -0.1262))
            .ReturnsAsync(expectedWeather);

        // Act
        var result = await _controller.GetWeatherByLatLonAsync(51.5002, -0.1262);

        // Assert
        result.Should().BeOfType(typeof(ActionResult<Weather>));
        result.Value.Should().BeEquivalentTo(expectedWeather);
    }
}
