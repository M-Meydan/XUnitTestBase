using Microsoft.Extensions.DependencyInjection;

namespace XUnitTestBase.Extensions;

public static class ServiceCollectionExtensions
{

    /// <summary>
    /// Removes all existing registrations of <typeparamref name="TService"/>
    /// and adds a new one using <typeparamref name="TImplementation"/>.
    /// </summary>
    public static IServiceCollection ReplaceAll<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TService : class
        where TImplementation : class, TService
    {
        var toRemove = services
            .Where(s => s.ServiceType == typeof(TService))
            .ToList();

        foreach (var descriptor in toRemove)
            services.Remove(descriptor);

        services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
        return services;
    }

    /// <summary>
    /// Replaces an exact implementation of <typeparamref name="TService"/> mapped to <typeparamref name="TOldImpl"/>
    /// with <typeparamref name="TNewImpl"/>. Leaves other registrations of <typeparamref name="TService"/> intact.
    /// </summary>
    public static IServiceCollection ReplaceExact<TService, TOldImpl, TNewImpl>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TService : class
        where TOldImpl : class, TService
        where TNewImpl : class, TService
    {
        var matches = services
            .Where(s =>
                s.ServiceType == typeof(TService) &&
                s.ImplementationType == typeof(TOldImpl))
            .ToList();

        foreach (var descriptor in matches)
            services.Remove(descriptor);

        services.Add(new ServiceDescriptor(typeof(TService), typeof(TNewImpl), lifetime));
        return services;
    }

    /// <summary>
    /// Replaces the first matching registration of <typeparamref name="TService"/> with <typeparamref name="TImplementation"/>,
    /// or adds it if no matching implementation is registered. This does not remove all existing bindings.
    /// </summary>
    public static IServiceCollection AddOrReplace<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TService : class
        where TImplementation : class, TService
    {
        var firstMatch = services.FirstOrDefault(s =>
            s.ServiceType == typeof(TService) &&
            s.ImplementationType == typeof(TImplementation));

        if (firstMatch != null)
            services.Remove(firstMatch);

        services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
        return services;
    }
}
