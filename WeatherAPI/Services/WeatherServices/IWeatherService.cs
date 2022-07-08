using WeatherAPI.Models;

namespace WeatherAPI.Services.WeatherServices;
public interface IWeatherService
{
    Task<Weather> GetWeatherByLatLonAsync(double latitude, double longitude);
}