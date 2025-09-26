using Microsoft.Extensions.DependencyInjection;

namespace Wisp.Framework.Util.ServiceDiscovery;

/// <summary>
/// Marks a service that will be automatically added to the main DI container
/// </summary>
/// <param name="lifetime"></param>
[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Singleton) : Attribute
{
    public ServiceLifetime Lifetime { get; set; } = lifetime;
}