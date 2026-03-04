---
icon: lucide/syringe
---

# Dependency Injection

Wisp uses Microsoft's dependency injection (DI) container to manage services.
You can add or modify services at various points in the startup pipeline.

For general information about .NET dependency injection, see the
[.NET dependency injection documentation](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/overview).

## Adding Services

There are two "core" methods of adding services to the container.

**Directly using the `Services` property of `WispHostBuilder`:**

```csharp
var hostBuilder = new WispHostBuilder();

hostBuilder.Services.AddSingleton<MyService>();
hostBuilder.Services.AddTransient<MyOtherService>();
```

**Using the `ConfigureServices` method of `WispHostBuilder`:**

```csharp
var hostBuilder = new WispHostBuilder();

hostBuilder.ConfigureServices(services => {
    services.AddSingleton<MyService>();
    services.AddTransient<MyOtherService>();
});
```

Once registered, services can be injected into other components.

Each service must have one of three lifetime options:

 - `Singleton` - one instance for the lifetime of the application
 - `Scoped` - one instance per scope (typically per request)
 - `Transient` - a new instance each time it is requested

## Injecting Dependencies

### Automatically

Any type instantiated through the DI container will have its dependencies injected. The DI container injects dependencies using constructor arguments. Both
explicit and primary constructors are supported.

**Primary Constructor:**

```csharp
public class MyService(AppDbContext db) {}
```

**Explicit Constructor:**

```csharp
public class MyService 
{
    public MyService(AppDbContext db)
    {}
}
```

### Manually

**Manual resolution should generally be avoided unless there is a clear and justified need.**

If you need to get services dynamically or you need to manually manage scoping, you
can ask the DI container to inject an instance of `IServiceProvider` and then
interact with it manually.

#### Dynamic Resolution

```csharp
public class MyDynamicService(IServiceProvider serviceProvider)
{
    public void DoSomething(Type type)
    {
        var instance = serviceProvider.GetService(type);

        if (instance is MyService ms) 
        {
            ms.DoAThing();
        }
    }
}
```

#### Manual Scoping

```csharp
public class MyScopedService(IServiceProvider serviceProvider)
{
    public async Task<Something> GetSomethingAsync()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var scopedInstance = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await scopedInstance.GetSomethingAsync();
    }
}
```

## Service Discovery

You can enable automatic discovery of services. When this option is enabled, Wisp will scan the provided assembly for types annotated with the `[Service]` attribute on startup and automatically add them to the DI container.

The ServiceAttribute has one optional argument: `ServiceLifetime`. The default is `ServiceLifetime.Singleton`.

Services registered through auto-discovery get the same treatment as services
registered manually.

**Service auto-discovery must be manually enabled**

```csharp
hostBuilder.UseServiceDiscovery(Assembly.GetExecutingAssembly());
```

## Advanced Usage

### Resolving Services During Configuration

If you need to get an instance of a registered service during the `WispHostBuilder`
stage, before the main `ServiceProvider` is built, you can call `WispHostBuilder.GetServiceProvider()` to build a temporary service provider containing all services registered up to that moment.

!!! warning
    This temporary instance of `IServiceProvider` only contains services that have been
    added to the service collection before creating it.

    Importantly, it **does not** contain services added through auto-discovery!

    This functionality can be useful, but it is also a significant footgun. Unless
    you can articulate a very good reason for using this, you're probably doing
    something wrong.

```csharp
var hostBuilder = new WispHostBuilder();

hostBuilder.Services.AddSingleton<MyService>();

var myServiceInstance = hostBuilder.GetServiceProvider().GetRequiredService<MyService>()
```

### Late Registration

In some edge cases you may need to register services at the very end of the
initialization phase, immediately before the final `IServiceProvider` is built during `WispHostBuilder.Build()`.
This is useful when a registration requires resolving services that have already
been added to the container.

You can use the `WispHostBuilder.AddDoLast` method to register a callback to do just that.

For example, [this mechanism is used](https://github.com/WispFramework/Wisp/commit/72586c31716dcfe9d053fd8af92f91bac8ced629#diff-6fc2ece7cd1c82e0fd71a6f6cb03204b564cbed4346f55c2de3e8e70d765f45aR12-R20) 
in the [L18n extension](../../Wisp_Extensions/Localization/about/) to register template filters that depend on services that must already exist in the container.

The `AddDoLast` method accepts a function that takes one parameter of the type `WispHostBuilder`.
This gives the callback access to the entire builder, not just the service collection.

Note that `GetServiceProvider()` creates a temporary service provider containing
all services registered so far.

**Example:**

```csharp
var hostBuilder = new WispHostBuilder();

hostBuilder.AddDoLast(builder => {
    var instance = builder.GetServiceProvider().GetRequiredService<TemplateFilters>();

    builder.AddTemplateFilter("demo", instance.DemoFilter);
});
```