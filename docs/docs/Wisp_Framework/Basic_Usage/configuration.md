---
icon: lucide/cog
---

# Configuration

Wisp provides configuration support for both framework behavior and application settings. 
This page describes the available configuration settings and how to customize them.

---

## Configuration Sources

Wisp uses the `Microsoft.Extensions.Configuration` package for loading and managing configuration. By default, it
includes the JSON configuration provider. You can extend this by adding additional configuration providers.

Wisp includes a built-in default configuration embedded in the framework assembly.
If a `wisp.json` file exists in the application root, it will override those defaults.
You can extend or override this by providing additional configuration sources via the `WispHostBuilder.Configure` method.

Configuration sources are applied in the order they are added, with later sources overriding earlier ones.

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

The framework expects a configuration section named `Wisp` that maps to the `WispConfiguration` class. This section contains key runtime settings.

The `Wisp` configuration section has sensible defaults and does not need to be defined unless you want to override specific values.

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
| Host           | string | IP address to bind the HTTP server to | `127.0.0.1`   |
| Port           | int    | TCP port for the HTTP server          | `6969`        |
| LogLevel       | string | Logging Level                         | `Information` |
| StaticFileRoot | string | Root directory for static files       | `wwwroot`     |

---

## Dependency Injection Configuration

Wisp uses Microsoft's dependency injection (DI) container to manage services.  
You can add or modify services during startup using `WispHostBuilder.ConfigureServices`.

See [Dependency Injection](dependency-injection) for more details.

Example:

```csharp
var builder = new WispHostBuilder();

builder.Services.AddSingleton<IMyService, MyService>();

// The `builder.Configuration` property exposes the underlying `IConfiguration` instance.
builder.Services.Configure<MySettings>(builder.Configuration.GetRequiredSection("MySettings"));
```

This allows you to:

- Register your own services
- Configure options bound to configuration sections
- Override default framework services if necessary

### Service auto-discovery

You can enable automatic discovery of services. When this option is enabled, Wisp will scan for compatible types
with a `[Service]` attribute on startup and automatically add them to the DI container.

See [Dependency Injection](dependency-injection#service-discovery) for more details.

**Enable Service Discovery**

```csharp
hostBuilder.UseServiceDiscovery(Assembly.GetExecutingAssembly());
```

**Register a Service**

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

Your logging configuration is applied after the framework defaults, allowing you to extend or override them.

---

## Router and HTTP Server Configuration

- The router and HTTP server are registered as singletons in DI.
- The HTTP server listens on the IP and port specified in the configuration (`Host` and `Port`).
- **Advanced:** You can replace the HTTP server implementation by overriding the `IHttpServer` service in `ConfigureServices`.

See [Routing](routing) for details.

---

### Templating Configuration

See [Templating](templating) for details.

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
- Do not store sensitive settings in source control. Use environment variables or a secure secrets manager.
