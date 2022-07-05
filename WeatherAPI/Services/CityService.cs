using Newtonsoft.Json.Linq;
using WeatherAPI.Models;

namespace WeatherAPI.Services
{
    public class CityService : ICityService
    {
        public readonly HttpClient _httpClient;
        public readonly CityContext _cityContext;
        
        public CityService(CityContext cityContext) 
        {
            _cityContext = cityContext;
            _httpClient = new HttpClient();
        }
        public async Task<List<City>> GetAllCity() 
        {
            return _cityContext.Cities.ToList();
        }

        public async Task<bool> CityExists(string cityName) 
        {
            return _cityContext.Cities.Any(b => b.name == cityName);
        }

        public async Task<City> AddCityAsync(City city) {
            _cityContext.Add(city);
            _cityContext.SaveChanges();
            return city;            
        }

        public async Task<City> UpdateCityAsync(City city)
        {
            var existingCityFound = await FindCityByNameAsync(city.name);

            existingCityFound.name = city.name;
            existingCityFound.latitude = city.latitude;
            existingCityFound.longitude = city.longitude;
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
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            string responseString = await response.Content.ReadAsStringAsync();

            JObject responseObject = JObject.Parse(responseString);
            var responseObjectResults = responseObject["results"];

            if (!response.IsSuccessStatusCode || responseObjectResults is null)
                throw new HttpRequestException($"No geocoding found for {cityName}, " +
                    $"please do POST request to /api/v1/city endpoint to add new city.");

            var firstLocation = responseObject["results"].First();

            City city = new City()
            {
                name = firstLocation["name"].ToString(),
                latitude = double.Parse(firstLocation["latitude"].ToString()),
                longitude = double.Parse(firstLocation["longitude"].ToString())
            };

            city = await AddCityAsync(city);
            return city;
        }

        private async Task<City> FindCityByNameAsync(string cityName)
        {
            var city = _cityContext.Cities.SingleOrDefault(x => x.name == cityName);
            return city;
        }
    }
}
