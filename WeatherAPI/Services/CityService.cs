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


            var firstLocation = responseObject["results"].First();
            Console.WriteLine();

            City city = new City()
            {
                name = firstLocation["name"].ToString(),
                latitude = double.Parse(firstLocation["latitude"].ToString()),
                longitude = double.Parse(firstLocation["longitude"].ToString())
            };

            return city;           
        }
    }
}
