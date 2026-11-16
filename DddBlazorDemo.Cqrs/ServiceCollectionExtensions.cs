using Microsoft.Extensions.DependencyInjection;

namespace DddBlazorDemo.Cqrs;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the dispatcher. Handlers and the unit of work are registered by the
    /// application and infrastructure layers; the dispatcher resolves them per operation.
    /// </summary>
    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.AddSingleton<IDispatcher, Dispatcher>();
        return services;
    }
}
