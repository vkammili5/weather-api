namespace WeatherAPI.Models;

public class Weather
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public WeatherCode WeatherCode { get; set; }
    public string WhatToPrepare { get; set; }
}

public enum WeatherCode
{
    ClearSky,
    Cloudy,
    Fog,
    RainOrWorse
}