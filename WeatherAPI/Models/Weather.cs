namespace WeatherAPI.Models;

public class Weather
{
    public double latitude { get; set; }
    public double longitude { get; set; }
    public DateTime startDate { get; set; }
    public DateTime endDate { get; set; }
    public List<WeatherCode> weatherCodes { get; set; }
}

public enum WeatherCode
{
    ClearSky,
    Cloudy,
    Fog,
    RainOrWorse
}