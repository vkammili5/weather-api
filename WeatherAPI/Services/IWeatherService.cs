using WeatherAPI.Models;

namespace WeatherAPI.Services;
public interface IWeatherService
{
    Task<Weather> GetWeatherByLatLonAsync(double latitude, double longitude);
}