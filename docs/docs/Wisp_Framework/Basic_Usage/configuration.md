---
icon: lucide/cog
---

# Configuration

Wisp provides flexible configuration options to tailor the framework and your application’s behavior. This page outlines
the available configuration settings and how to customize them.

---

## Configuration Sources

Wisp uses the `Microsoft.Extensions.Configuration` package for loading and managing configuration. It also, by default,
includes the JSON extension that allows loading JSON files. You can optionally extend this by adding other source type 
packages.

By default, Wisp loads configuration from a JSON file named `wisp.json` located in the application root.  
You can extend or override this by providing additional configuration sources via the `WispHostBuilder.Configure` method.

Example:

```csharp
var builder = new WispHostBuilder()
    .Configure(configBuilder =>
    {
        configBuilder.AddEnvironmentVariables();
        configBuilder.AddJsonFile("appsettings.local.json", optional: true);
    });
```

---

## Core Configuration Section: `Wisp`

The framework expects a configuration section named `Wisp` that maps to the `WispConfiguration` class. This section
contains key runtime settings.

### Typical `wisp.json` example:

```json
{
  "Wisp": {
    "Host": "127.0.0.1",
    "Port": 6969,
    "LogLevel": "Information",
    "StaticFileRoot": "wwwroot"
  }
}
```

### Configuration options in the `Wisp` section

| Setting        | Type   | Description                           | Default       |
|----------------|--------|---------------------------------------|---------------|
| Host           | string | IP address to bind the HTTP server to | `'127.0.0.1'` |
| Port           | int    | TCP port for the HTTP server          | `6969`        |
| LogLevel       | string | Logging Level                         | `Information` |
| StaticFileRoot | string | Root directory for static files       | `wwwroot`     |

---

## Dependency Injection Configuration

Wisp uses Microsoft's dependency injection (DI) container to manage services.  
You can add or modify services during startup using `WispHostBuilder.ConfigureServices`.

Example:

```csharp
var builder = new WispHostBuilder()
    .ConfigureServices(services =>
    {
        services.AddSingleton<IMyService, MyService>();
        services.Configure<MySettings>(config.GetSection("MySettings"));
    });
```

This allows you to:

- Register your own services
- Configure options bound to configuration sections
- Override default framework services if necessary

### Service auto-discovery

You can enable automatic discovery of services. When this option is enabled, Wisp will scan for compatible types
with a `[Service]` attribute on startup and automatically add them to the DI container.

```#!csharp hostBuilder.UseServiceDiscovery(Assembly.GetExecutingAssembly());```

```csharp
[Service(ServiceLifetime.Scoped)]
public class MyService {}
```

The `ServiceAttribute` has one optional argument: `ServiceLifetime`. The default is `ServiceLifetime.Singleton`.

---

## Logging Configuration

Wisp supports flexible logging configuration via `WispHostBuilder.ConfigureLogging`.  
By default, it adds a simple console logger.

Example of overriding or extending the default logging setup:

```csharp
var builder = new WispHostBuilder()
    .ConfigureLogging(logging =>
    {
        logging.SetMinimumLevel(LogLevel.Information);
        logging.AddFilter("MyApp.Namespace", LogLevel.Warning);
        logging.AddDebug();
    });
```

Behind the scenes, this merges your logging configuration with the defaults, ensuring sensible logging out of the box but giving you full control.

---

## Router and HTTP Server Configuration

- The router and HTTP server are registered as singletons in DI.
- The HTTP server listens on the IP and port specified in the configuration (`Host` and `Port`).
- You can replace the HTTP server implementation by overriding the `IHttpServer` service in `ConfigureServices`.

---

### Templating Configuration

!!! warning
    This feature is not implemented yet.

You can configure the templating engine using the `WispHostBuilder.ConfigureTemplates` extension method.

---

## Summary of Configuration Extension Points

| Extension Point               | Method                                   | Description                                         |
|-------------------------------|------------------------------------------|-----------------------------------------------------|
| Add custom config sources     | `WispHostBuilder.Configure`              | Add JSON, environment variables, etc.               |
| Register or override services | `WispHostBuilder.ConfigureServices`      | Add your own services or replace defaults           |
| Customize logging             | `WispHostBuilder.ConfigureLogging`       | Add, remove or adjust logging providers and filters |
| Enable controllers            | `WispApplicationBuilder.UseControllers`  | Registers controllers automatically                 |
| Add routes manually           | `WispApplicationBuilder.ConfigureRoutes` | Define lightweight routes                           |

---

## Additional Notes

- Configuration is immutable once `Build()` is called on the host builder.
- Access configuration values anywhere via DI using `IConfiguration` or bound options classes.
- Make sure to keep sensitive settings out of source control and use secure secrets management where appropriate.
