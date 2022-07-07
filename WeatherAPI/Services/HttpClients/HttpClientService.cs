using Newtonsoft.Json.Linq;

namespace WeatherAPI.Services.HttpClients;

public class HttpClientService : IHttpClientService
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public async Task<(string, bool)> GetAsync(string url)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(url);

        string responseString = await response.Content.ReadAsStringAsync();

        JObject responseObject = JObject.Parse(responseString);

        if (!response.IsSuccessStatusCode)
        {
            string errorReason = responseObject["reason"]!.ToString();
            return (errorReason, false);
        }

        bool isGeoApiFail = responseObject["latitude"] is null && responseObject["results"] is null;
        if (isGeoApiFail)
        {
            return ("", false);
        }

        return (responseString, true);
    }
}
