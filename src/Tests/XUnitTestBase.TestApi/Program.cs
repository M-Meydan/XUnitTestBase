using XUnitTestBase.TestApi.Services;

namespace XUnitTestBase.TestApi;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddScoped<ITimeService, RealTimeService>();
        builder.Services.AddHttpClient<IWeatherClient, WeatherClient>();

        var app = builder.Build();

        app.MapGet("/api/time", (ITimeService timeService) =>
            Results.Ok(new { Utc = timeService.GetCurrentTime() }));

        app.MapGet("/api/weather", async (IWeatherClient client) =>
        {
            var temp = await client.GetCurrentTempAsync();
            return Results.Ok(temp);
        });

        app.Run();
    }
}
