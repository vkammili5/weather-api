using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using WeatherAPI.Models;

namespace WeatherAPI.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;

    public WeatherService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<Weather> GetWeatherByLatLonAsync(double latitude, double longitude)
    {
        string url = "https://api.open-meteo.com/v1/forecast?" +
            $"latitude={latitude}&" +
            $"longitude={longitude}&" +
            "daily=weathercode,temperature_2m_max,temperature_2m_min&" +
            "timezone=Europe%2FLondon";

        HttpResponseMessage response = await _httpClient.GetAsync(url);

        string responseString = await response.Content.ReadAsStringAsync();

        JObject responseObject = JObject.Parse(responseString);

        var a = responseObject["daily"]["weathercode"].ToList();

        List<WeatherCode> listOfWeatherCode = new();
        foreach (var x in a)
        {
            int codeNumber = int.Parse(x.ToString());
            WeatherCode weatherCode = codeNumber switch
            {
                0 => WeatherCode.ClearSky,
                <= 3 => WeatherCode.Cloudy,
                < 51 => WeatherCode.Fog,
                _ => WeatherCode.RainOrWorse
            };

            listOfWeatherCode.Add(weatherCode);
        }

        Weather weather = new Weather()
        {
            latitude = double.Parse(responseObject["latitude"].ToString()),
            longitude = double.Parse(responseObject["longitude"].ToString()),
            startDate = DateTime.Parse(responseObject["daily"]["time"].First().ToString()),
            endDate = DateTime.Parse(responseObject["daily"]["time"].Last().ToString()),
            weatherCodes = listOfWeatherCode
        };

        return weather;
    }
}
