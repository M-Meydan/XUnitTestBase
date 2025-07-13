using Microsoft.Extensions.DependencyInjection;

namespace XUnitTestBase.Extensions;


/// <summary>
/// Replaces all registrations of <typeparamref name="TClient"/> with a test-safe
/// <see cref="HttpClient"/> configured using <typeparamref name="TFakeHandler"/>.
/// Intended for mocking typed clients in integration tests.
/// </summary>
/// <typeparam name="TClient">The client interface or type.</typeparam>
/// <typeparam name="TImplementation">The real implementation being replaced.</typeparam>
/// <typeparam name="TFakeHandler">A fake <see cref="HttpMessageHandler"/> used for testing.</typeparam>
/// <example>
/// services.ReplaceHttpClient&lt;IMyClient, MyClient, FakeHandler&gt;();
/// </example>
public static class HttpClientTestExtensions
{
    public static IServiceCollection ReplaceHttpClient<TClient, TImplementation, TFakeHandler>(
        this IServiceCollection services)
        where TClient : class
        where TImplementation : class, TClient
        where TFakeHandler : HttpMessageHandler, new()
    {
        // Remove all existing registrations of TClient
        var matches = services
            .Where(s => s.ServiceType == typeof(TClient))
            .ToList();

        foreach (var descriptor in matches)
            services.Remove(descriptor);

        // Register the typed client using the fake handler
        services.AddHttpClient<TClient, TImplementation>()
                .ConfigurePrimaryHttpMessageHandler(() => new TFakeHandler());

        return services;
    }
}
