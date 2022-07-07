namespace WeatherAPI.Models;

public class Weather
{
    public double latitude { get; set; }
    public double longitude { get; set; }
    public WeatherCode weatherCode { get; set; }
    public string whatToPrepare { get; set; }
}

public enum WeatherCode
{
    ClearSky,
    Cloudy,
    Fog,
    RainOrWorse
}