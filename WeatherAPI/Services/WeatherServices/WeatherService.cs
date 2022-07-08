using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;
using WeatherAPI.Models;
using WeatherAPI.Services.HttpClients;

namespace WeatherAPI.Services.WeatherServices;

public class WeatherService : IWeatherService
{
    private readonly IHttpClientService _httpClientService;

    public WeatherService(IHttpClientService httpClientService)
    {
        _httpClientService = httpClientService;
    }

    public async Task<Weather> GetWeatherByLatLonAsync(double latitude, double longitude)
    {
        string url = "https://api.open-meteo.com/v1/forecast?" +
            $"latitude={latitude}&" +
            $"longitude={longitude}&" +
            "daily=weathercode&" +
            "timezone=Europe%2FLondon";

        (string responseString, bool isSuccess) = await _httpClientService.GetAsync(url);

        if (!isSuccess) 
            throw new HttpRequestException(responseString);

        Weather weather = ParseJsonStringToWeather(responseString);

        return weather;
    }

    private static Weather ParseJsonStringToWeather(string responseString)
    {
        JObject responseObject = JObject.Parse(responseString);

        var weatherCodeJTokenList = responseObject["daily"]!["weathercode"]!.ToList();

        int weatherCodeNumber = int.Parse(weatherCodeJTokenList.First().ToString());
        WeatherCode weatherCode = NumberToWeatherCode(weatherCodeNumber);

        return new()
        {
            Latitude = double.Parse(responseObject["latitude"]!.ToString()),
            Longitude = double.Parse(responseObject["longitude"]!.ToString()),
            WeatherCode = weatherCode,
            WhatToPrepare = GetWhatToPrepare(weatherCode)
        };
    }

    private static WeatherCode NumberToWeatherCode(int weatherCodeNumber)
    {
        return weatherCodeNumber switch
        {
            <= 1 => WeatherCode.ClearSky,
            <= 3 => WeatherCode.Cloudy,
            < 51 => WeatherCode.Fog,
            _ => WeatherCode.RainOrWorse
        };
    }

    private static string GetWhatToPrepare(WeatherCode weatherCode)
    {
        return weatherCode switch
        {
            WeatherCode.ClearSky => "wear summer clothings, wear a cap, apply sunscreen",
            WeatherCode.Fog => "bring flash light",
            WeatherCode.Cloudy => "bring coat",
            WeatherCode.RainOrWorse => "bring umbrella and rain coat",
            _ => "wear normal clothes"
        };
    }
}
