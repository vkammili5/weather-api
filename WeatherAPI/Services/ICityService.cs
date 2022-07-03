using WeatherAPI.Models;

namespace WeatherAPI.Services
{
    public interface ICityService
    {
        Task<City> GetWeatherByCityAsync(string cityName);
    }
}