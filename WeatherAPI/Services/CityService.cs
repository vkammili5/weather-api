using Newtonsoft.Json.Linq;
using WeatherAPI.Models;

namespace WeatherAPI.Services
{
    public class CityService : ICityService
    {
        public readonly HttpClient _httpClient;
        public CityService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<City> GetCityByCityNameAsync(string cityName)
        {
            string url = "https://geocoding-api.open-meteo.com/v1/search?" +
                            $"name={cityName}&";
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            string responseString = await response.Content.ReadAsStringAsync();

            JObject responseObject = JObject.Parse(responseString);
            var responseObjectResults = responseObject["results"];

            if (!response.IsSuccessStatusCode || responseObjectResults is null)
            {
                throw new HttpRequestException($"No geocoding found for {cityName}");
            }

            var firstLocation = responseObjectResults.First();
            Console.WriteLine();

            City city = new City()
            {
                name = firstLocation["name"].ToString(),
                latitude = double.Parse(firstLocation["latitude"].ToString()),
                longitude = double.Parse(firstLocation["longitude"].ToString())
            };

            return city;           
        }

        public Task<City> AddCityAsync(City newCity)
        {
            throw new NotImplementedException();
        }

        public Task<City> UpdateCityAsync(City city)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CityExists(string name)
        {
            throw new NotImplementedException();
        }
    }
}
