using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Wisp.Framework.Util.ServiceDiscovery;

public class ServiceRegistrar(IServiceCollection serviceCollection, ILogger<ServiceRegistrar> log)
{
    public void ScanAssembly(Assembly assembly)
    {
        log.LogInformation("scanning for services in {Assembly}", assembly.FullName);
        
        var types = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<ServiceAttribute>() != null);

        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<ServiceAttribute>()!;
            
            log.LogInformation("Adding {Type} as a service with {Lifetime} lifetime", type.Name, attr.Lifetime);

            switch (attr.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    serviceCollection.AddSingleton(type);
                    break;
                case ServiceLifetime.Scoped:
                    serviceCollection.AddScoped(type);
                    break;
                case ServiceLifetime.Transient:
                    serviceCollection.AddTransient(type);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}