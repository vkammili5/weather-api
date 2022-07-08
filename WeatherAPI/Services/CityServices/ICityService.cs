using WeatherAPI.Models;

namespace WeatherAPI.Services.CityServices
{
    public interface ICityService
    {
        Task<List<City>> GetAllCity();
        Task<City> GetCityByCityNameAsync(string cityName);
        Task<City> AddCityAsync(City city);
        Task<City> UpdateCityAsync(City cityToUpdate);
        Task DeleteCityAsync(string cityName);
        Task<bool> CityExists(string cityName);
    }
}