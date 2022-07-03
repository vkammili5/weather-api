using WeatherAPI.Models;

namespace WeatherAPI.Services
{
    public interface ICityService
    {
        Task<City> GetCityByCityNameAsync(string cityName);
        Task<City> AddCityAsync(City newCity);
        Task<bool> CityExists(string name);
    }
}