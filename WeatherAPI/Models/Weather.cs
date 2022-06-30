namespace WeatherAPI.Models;

public class Weather
{
    public double latitude { get; set; }
    public double longitude { get; set; }
    //public DateTime StartDate { get; set; }
    //public DateTime EndDate { get; set; }
    //public List<WeatherCode> WeatherCodes { get; set; }
}

public enum WeatherCode
{
    ClearSky,
    Cloudy,
    Fog,
    RainOrWorse
}