using WeatherAPI.Models;

namespace WeatherAPI.Services
{
    public interface ICityService
    {
        Task<City> GetCityByCityNameAsync(string cityName);
        Task<City> AddCityAsync(City newCity);
        Task<City> UpdateCityAsync(City cityToUpdate);
        Task DeleteCityAsync(string cityName);
        Task<bool> CityExists(string name);
    }
}