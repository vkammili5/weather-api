using Newtonsoft.Json.Linq;
using WeatherAPI.Models;

namespace WeatherAPI.Services
{
    public class CityService : ICityService
    {
        public readonly HttpClient _httpClient;
        public readonly CityContext _cityContext;
        
        public CityService(CityContext cityContext) { 

            _cityContext = cityContext;
            _httpClient = new HttpClient();
        }
        public async Task<City> FindCityByName(string cityName) { 
            var city = await _cityContext.Cities.FindAsync(cityName);
            return city;
        }
        public async Task<bool> CityExist(string cityName) {

            return _cityContext.Cities.Any(b => b.name == cityName);

        }
        public async Task<City> AddCityAsync(City city) {
            _cityContext.Add(city);
            _cityContext.SaveChanges();
             return city;            
        }
        public async Task<City> UpdateCityAsync(string cityName, City city)
        {
            var existingCityFound = await FindCityByName(cityName);
            try
            {                
                if (existingCityFound != null)
                {
                    existingCityFound.name = city.name;
                    existingCityFound.latitude = city.latitude;
                    existingCityFound.longitude = city.longitude;
                    _cityContext.SaveChanges();
                    return existingCityFound;
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
           
            return existingCityFound;
        }
        public async Task<bool> DeleteCityAsync(string cityName) {

            var existingCityFound = FindCityByName(cityName);
            bool success = false;
            
            if (existingCityFound != null)
            {
                _cityContext.Remove(cityName);
                _cityContext.SaveChanges();
                success = true;
            }

            return success;
        }     
       
        public async Task<City> GetCityByCityNameAsync(string cityName)
        {
            if (await CityExist(cityName)) {
                City city = await FindCityByName(cityName);
                return city;
            }
            else
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
}
