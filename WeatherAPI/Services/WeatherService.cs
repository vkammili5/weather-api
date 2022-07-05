using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;
using WeatherAPI.Models;
using WeatherAPI.Services.HttpClients;

namespace WeatherAPI.Services;

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
        {
            throw new HttpRequestException(responseString);
        }

        Weather weather = ParseJsonStringToWeather(responseString);

        return weather;
    }

    private static Weather ParseJsonStringToWeather(string responseString)
    {
        JObject responseObject = JObject.Parse(responseString);

        var weatherCodeJTokenList = responseObject["daily"]!["weathercode"]!.ToList();

        List<WeatherCode> weatherCodeList = weatherCodeJTokenList.Select(x =>
        {
            int weatherCodeNumber = int.Parse(x.ToString());
            return NumberToWeatherCode(weatherCodeNumber);
        }).ToList();

        Weather weather = ResponseObjectToWeather(responseObject, weatherCodeList);
        return weather;
    }

    private static Weather ResponseObjectToWeather(JObject responseObject, List<WeatherCode> weatherCodeList)
    {
        return new()
        {
            latitude = double.Parse(responseObject["latitude"]!.ToString()),
            longitude = double.Parse(responseObject["longitude"]!.ToString()),
            startDate = DateTime.Parse(responseObject["daily"]!["time"]!.First().ToString()),
            endDate = DateTime.Parse(responseObject["daily"]!["time"]!.Last().ToString()),
            weatherCodes = weatherCodeList
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
}
