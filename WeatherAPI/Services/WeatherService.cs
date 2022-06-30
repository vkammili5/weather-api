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

    public async Task<Weather> GetWeatherByLatLon(double latitude, double longitude)
    {
        string url = "https://api.open-meteo.com/v1/forecast?" +
            $"latitude={latitude}&" +
            $"longitude={longitude}&" +
            "daily=weathercode,temperature_2m_max,temperature_2m_min&" +
            "timezone=Europe%2FLondon";

        HttpResponseMessage response = await _httpClient.GetAsync(url);

        string responseString = await response.Content.ReadAsStringAsync();

        Weather? weather =
                JsonSerializer.Deserialize<Weather>(responseString);

        return weather;
    }
}
