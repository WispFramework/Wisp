---
icon: lucide/server-cog
---

# Background Services

A background service is a service that runs in the background, independent of the traditional
request-response HTTP model.

Wisp injects dependencies and takes care of managing your service's lifecycle for you.

## Writing a Background Service

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
        try 
        {
            while(!cancellationToken.IsCancellationRequested) 
            {
                await hws.SayHelloWorldAsync();
                await Task.Delay(TimeSpan.FromSeconds(5))
            }    
        }
        catch (TaskCanceledException) {}        
    }
}
```

## Long-Running Services

Wisp will instantiate your service, inject arguments and them call `RunAsync` exactly once when the application starts,
and then cancel the [`CancellationToken`](https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/task-cancellation)
when the application is exiting.

If you need your service to keep running in the background, it is up to you to do so. The recommended way of achieving
this is a `while` loop that keeps running until the token is canceled. Don't forget to wrap this in a `try/catch` and
handle the `TaskCanceledException`, which, in this case, does not indicate an error.

## Registering a Background Service

Use the `AddBackgroundService<T>` extension method.

```csharp
var hostBuilder = new new WispHostBuilder();

hostBuilder.AddBackgroundService<HelloWorldBackgroundService>();

var appBuilder = hostBuilder.Build();
var app = appBuilder.Build();

await app.RunAsync();

```

## Starting Background Services

Background services start automatically on application start.

## Stopping Background Services

Wisp will automatically cancel the `CancellationToken` passed to `RunAsync` when the application is shutting down. If you
need to be able to stop the service from elsewhere, you can pass on the cancellation token instance from within `RunAsync`.

Passing your own `CancellationToken` to `RunAsync` is currently not supported.