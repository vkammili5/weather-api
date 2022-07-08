using Newtonsoft.Json.Linq;
using WeatherAPI.Models;
using WeatherAPI.Services.HttpClients;

namespace WeatherAPI.Services.CityServices
{
    public class CityService : ICityService
    {
        public readonly IHttpClientService _httpClientService;
        public readonly CityContext _cityContext;

        public CityService(CityContext cityContext, IHttpClientService httpClientService)
        {
            _cityContext = cityContext;
            _httpClientService = httpClientService;
        }

        public async Task<List<City>> GetAllCity()
        {
            return _cityContext.Cities.ToList();
        }

        public async Task<bool> CityExists(string cityName)
        {
            return _cityContext.Cities.Any(b => b.Name.ToLower() == cityName.ToLower());
        }

        public async Task<City> AddCityAsync(City city)
        {
            _cityContext.Add(city);
            _cityContext.SaveChanges();
            return city;
        }

        public async Task<City> UpdateCityAsync(City city)
        {
            var existingCityFound = await FindCityByNameAsync(city.Name);

            existingCityFound.Name = city.Name;
            existingCityFound.Latitude = city.Latitude;
            existingCityFound.Longitude = city.Longitude;
            _cityContext.SaveChanges();
            return existingCityFound;
        }

        public async Task DeleteCityAsync(string cityName)
        {
            var existingCityFound = await FindCityByNameAsync(cityName);

            _cityContext.Remove(existingCityFound);
            _cityContext.SaveChanges();
        }

        public async Task<City> GetCityByCityNameAsync(string cityName)
        {
            if (await CityExists(cityName))
                return await FindCityByNameAsync(cityName);

            return await GetCityOnPublicApiAsync(cityName);
        }

        private async Task<City> GetCityOnPublicApiAsync(string cityName)
        {
            string url = "https://geocoding-api.open-meteo.com/v1/search?" +
                                            $"name={cityName}";

            (string responseString, bool isSuccess) = await _httpClientService.GetAsync(url);
            if (!isSuccess)
            {
                throw new HttpRequestException($"No geocoding found for {cityName}, " +
                    $"please do POST request to /api/v1/city endpoint to add new city.");
            }

            City city = await parseJsonStringToCity(responseString);
            city = await AddCityAsync(city);
            return city;
        }

        private static async Task<City> parseJsonStringToCity(string responseString)
        {
            JObject responseObject = JObject.Parse(responseString);

            var firstLocation = responseObject["results"].First();

            City city = new City()
            {
                Name = firstLocation["name"].ToString(),
                Latitude = double.Parse(firstLocation["latitude"].ToString()),
                Longitude = double.Parse(firstLocation["longitude"].ToString())
            };
            return city;
        }

        private async Task<City> FindCityByNameAsync(string cityName)
        {
            var city = _cityContext.Cities.SingleOrDefault(x => x.Name.ToLower() == cityName.ToLower());
            return city;
        }
    }
}
