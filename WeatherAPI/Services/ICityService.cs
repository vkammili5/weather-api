using WeatherAPI.Models;

namespace WeatherAPI.Services
{
    public interface ICityService
    {
        Task<City> GetCityByCityNameAsync(string cityName);
        Task<City> FindCityByName(string cityName);

       // Task DeleteCityAsync(string cityName);

       // Task<City> UpdateCityAsync(string cityName, City city);
        Task<City> AddCityAsync(City city);

        Task<bool> CityExist(string cityName);
    }
}