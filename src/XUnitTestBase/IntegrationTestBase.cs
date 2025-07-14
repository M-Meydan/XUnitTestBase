using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace XUnitTestBase;

/// <summary>
/// Base class for API integration tests using WebApplicationFactory.
/// Allows overriding services for test-specific setups.
/// 
/// Features:
/// - Provides a shared HttpClient instance (`Client`) scoped to the test server.
/// - Allows dependency injection overrides via the `OverrideServices` method.
/// - Exposes resolved services via `GetService<T>()`.
/// - Disposes factory, client, and scope automatically.
/// - Supports mocking HTTP clients using `AddFakeHttpClient` extension.
/// </summary>
/// <typeparam name="TEntryPoint">The app's Program or Startup class.</typeparam>
/// <example>
/// public class UserApiTests : IntegrationTestBase<Program>
/// {
///     protected override Action<IServiceCollection>? OverrideServices() =>
///         services => services.AddScoped<IMyService, FakeMyService>();
/// 
///     [Fact]
///     public async Task Get_ReturnsOk()
///     {
///         var response = await Client.GetAsync("/api/users");
///         response.EnsureSuccessStatusCode();
///     }
/// }
/// </example>
public abstract class IntegrationTestBase<TEntryPoint> : IDisposable
    where TEntryPoint : class
{
    protected readonly HttpClient Client;
    private readonly IServiceScope _scope;
    protected IServiceProvider Services => _scope.ServiceProvider;
    protected CustomWebApplicationFactory<TEntryPoint> Factory { get; }

    /// <summary>
    /// Override this method in your test class to register fake/mock services.
    /// </summary>
    /// <returns>Service override action.</returns>
    protected virtual Action<IServiceCollection>? OverrideServices() => null;

    /// <summary>
    /// Initializes the integration test base and applies optional service overrides.
    /// </summary>
    protected IntegrationTestBase()
    {
        Factory = new CustomWebApplicationFactoryWithOverrides(OverrideServices());
        Client = Factory.CreateClient();
        _scope = Factory.Services.CreateScope();
    }

    protected T GetService<T>() where T : notnull => Services.GetRequiredService<T>();

    public void Dispose()
    {
        _scope.Dispose();
        Client.Dispose();
        Factory.Dispose();
    }

    private class CustomWebApplicationFactoryWithOverrides : CustomWebApplicationFactory<TEntryPoint>
    {
        private readonly Action<IServiceCollection>? _overrides;

        public CustomWebApplicationFactoryWithOverrides(Action<IServiceCollection>? overrides)
        {
            _overrides = overrides;
        }

        protected override Action<IServiceCollection>? OverrideServices() => _overrides;
    }
}

public class CustomWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    /// <summary>
    /// Override this method to apply service overrides during test startup.
    /// </summary>
    protected virtual Action<IServiceCollection>? OverrideServices() => null;


    /// <summary>
    /// Override to customize IConfiguration loading (optional).
    /// </summary>
    protected virtual void ConfigureAppConfiguration(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            var projectDir = Directory.GetCurrentDirectory();

            configBuilder
                .SetBasePath(projectDir)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables();
        });

        builder.UseEnvironment("IntegrationTest");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Inject appsettings/Test config
        ConfigureAppConfiguration(builder);

        // Inject DI overrides
        builder.ConfigureServices(services =>
        {
            OverrideServices()?.Invoke(services);
        });
    }
}