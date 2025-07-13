namespace XUnitTestBase.TestApi.Services;

public interface IWeatherClient
{
    Task<string> GetCurrentTempAsync();
}

public class WeatherClient : IWeatherClient
{
    private readonly HttpClient _http;

    public WeatherClient(HttpClient http) => _http = http;

    public async Task<string> GetCurrentTempAsync()
    {
        // Normally hits external API
        var response = await _http.GetStringAsync("https://api.weather.local/temp");
        return response;
    }
}
