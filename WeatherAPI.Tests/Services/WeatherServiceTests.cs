using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherAPI.Models;
using WeatherAPI.Services.HttpClients;
using WeatherAPI.Services.WeatherServices;

namespace WeatherAPI.Tests.Services;
internal class WeatherServiceTests
{
    private WeatherService _service;
    private Mock<IHttpClientService> _httpClientService;

    [SetUp]
    public void Setup()
    {
        // Arrange
        _httpClientService = new Mock<IHttpClientService>();
        _service = new WeatherService(_httpClientService.Object);
    }

    [Test]
    public async Task GetWeatherByLatLonAsync_Should_Return_Correct_Response()
    {
        // Arrange
        string responseString = "{" +
            "\"latitude\":53.48," +
            "\"longitude\":-2.2400002," +
            "\"elevation\":48.0," +
            "\"generationtime_ms\":0.2720355987548828," +
            "\"utc_offset_seconds\":3600," +
            "\"daily\":{ " +
            "\"time\":[\"2022-07-05\",\"2022-07-06\",\"2022-07-07\",\"2022-07-08\",\"2022-07-09\",\"2022-07-10\",\"2022-07-11\"]," +
            "\"weathercode\":[0.0,3.0,3.0,3.0,3.0,45.0,45.0]}," +
            "\"daily_units\":{ \"time\":\"iso8601\",\"weathercode\":\"wmo code\"}}";

        string url = "https://api.open-meteo.com/v1/forecast?" +
            $"latitude={51.5002}&" +
            $"longitude={-0.1262}&" +
            "daily=weathercode&" +
            "timezone=Europe%2FLondon";

        _httpClientService.Setup(h => h.GetAsync(url))
            .ReturnsAsync((responseString, true));

        Weather expectedWeather = new Weather()
        {
            Latitude = 53.48,
            Longitude = -2.2400002,
            WeatherCode = WeatherCode.ClearSky,
            WhatToPrepare = "wear summer clothings, wear a cap, apply sunscreen"
        };

        // Act
        Weather result = await _service.GetWeatherByLatLonAsync(51.5002, -0.1262);

        // Assert
        result.Should().BeOfType(typeof(Weather));
        result.Should().BeEquivalentTo(expectedWeather);
    }

    [Test]
    public async Task GetWeatherByLatLonAsync_With_Invalid_LatLon_Should_Throw_Exception()
    {
        // Arrange
        string responseString = "Latitude must be in range of -90 to 90°. Given: 5100.5.";

        string url = "https://api.open-meteo.com/v1/forecast?" +
            $"latitude={5100.5002}&" +
            $"longitude={-0.1262}&" +
            "daily=weathercode&" +
            "timezone=Europe%2FLondon";

        _httpClientService.Setup(h => h.GetAsync(url))
            .ReturnsAsync((responseString, false));

        // Act
        Func<Task> act =_service.Awaiting(x => x.GetWeatherByLatLonAsync(5100.5002, -0.1262));

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage(responseString);
    }
}
