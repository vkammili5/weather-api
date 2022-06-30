using WeatherAPI.Models;

namespace WeatherAPI.Services;
public interface IWeatherService
{
    Task<Weather> GetWeatherByLatLon(double latitude, double longitude);
}