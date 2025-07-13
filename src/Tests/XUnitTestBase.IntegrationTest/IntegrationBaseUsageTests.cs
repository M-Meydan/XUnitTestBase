using Bogus.Bson;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using XUnitTestBase;
using XUnitTestBase.Extensions;
using XUnitTestBase.TestApi;
using XUnitTestBase.TestApi.Services;

/// <summary>
/// Demonstrates overriding services and clients using IntegrationTestBase:
/// - ReplaceAll for ITimeService
/// - ReplaceHttpClient for IWeatherClient
/// </summary>
/// <example>
/// services.ReplaceAll&lt;ITimeService, FakeTimeService&gt;();
/// services.ReplaceHttpClient&lt;IWeatherClient, WeatherClient, FakeWeatherHandler&gt;();
/// </example>
public class IntegrationBaseUsageTests : IntegrationTestBase<Program>
{
    protected override Action<IServiceCollection>? OverrideServices() =>
        services =>
        {
            services.ReplaceAll<ITimeService, FakeTimeService>();
            services.ReplaceHttpClient<IWeatherClient, WeatherClient, FakeWeatherHandler>();
        };

    [Fact]
    public void GetService_ReturnsFakeService()
    {
        var time = GetService<ITimeService>().GetCurrentTime();
        Assert.Equal("2000-01-01T00:00:00.0000000Z", time);
    }

    [Fact]
    public async Task HttpClient_UsesFakeHandler()
    {
        var response = await Client.GetAsync("/api/weather");
        var body = await response.Content.ReadAsStringAsync();
        var temperature = JsonSerializer.Deserialize<string>(body);
        Assert.Equal("\"72°F\"", temperature);
    }


    [Fact]
    public async Task Api_UsesOverriddenTimeService()
    {
        var response = await Client.GetAsync("/api/time");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<TimeDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.Equal("2000-01-01T00:00:00.0000000Z", result?.Utc);
    }

    private class TimeDto
    {
        public string Utc { get; set; } = string.Empty;
    }

    private class FakeTimeService : ITimeService
    {
        public string GetCurrentTime() => "2000-01-01T00:00:00.0000000Z";
    }

    private class FakeWeatherHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var content = new StringContent("\"72°F\"", Encoding.UTF8, "application/json");
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content });
        }
    }
}
