namespace WeatherAPI.Services.HttpClients;

public interface IHttpClientService
{
    Task<(string, bool)> GetAsync(string url);
    

}