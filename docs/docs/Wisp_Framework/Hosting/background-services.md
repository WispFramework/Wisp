---
icon: lucide/server-cog
---

# Background Services

A background service runs independently of the request-response HTTP lifecycle.

Wisp injects dependencies and takes care of managing your service's lifecycle for you.

## Writing a Background Service

!!! warning
    Background services are typically registered as singletons.
    Do not inject scoped services directly into a background service.
    If you need scoped dependencies (e.g., a database context), create a scope manually using
    `IServiceProvider.CreateScope()` inside `RunAsync`.

    ```csharp
    public class DatabaseBackgroundService(IServiceProvider serviceProvider)
    : IBackgroundService
    {
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                await db.DoWorkAsync(cancellationToken);

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }
    ```

A background service must implement the `IBackgroundService` interface. Any constructor arguments
will be injected.

```csharp
public class HelloWorldBackgroundService : IBackgroundService 
{
    private readonly HelloWorldService _hws;
    
    public HelloWorldBackgroundService(HelloWorldService hws)
    {
        // Injected from DI
        _hws = hws;
    }
    
    public async Task RunAsync(CancellationToken cancellationToken) 
    {
        while(!cancellationToken.IsCancellationRequested) 
        {
            await _hws.SayHelloWorldAsync();
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }
    }
}
```

If you pass the `CancellationToken` to awaited operations (such as `Task.Delay`), the loop will exit 
automatically when cancellation is requested. Explicitly catching `TaskCanceledException` is usually not required.

## Long-Running Services

Wisp instantiates your service and calls `RunAsync` once during application startup.
The method runs in the background and does not block the HTTP server from starting. It will then cancel the
[`CancellationToken`](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/task-cancellation)
when the application is exiting.

If your service needs to perform continuous work, use a loop that runs until the cancellation token is triggered.

## Registering a Background Service

Use the `AddBackgroundService<T>` extension method.

```csharp
var hostBuilder = new WispHostBuilder();

hostBuilder.AddBackgroundService<HelloWorldBackgroundService>();

var appBuilder = hostBuilder.Build();
var app = appBuilder.Build();

await app.RunAsync();

```

## Starting Background Services

Background services start automatically on application start.

## Stopping Background Services

Wisp will automatically cancel the `CancellationToken` passed to `RunAsync` when the application is shutting down.

If your background service coordinates other long-running operations, you should pass the provided 
`CancellationToken` to those operations so they can shut down gracefully.

Passing a custom `CancellationToken` to `RunAsync` is not currently supported.