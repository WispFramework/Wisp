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
    
    public helloWorldBackgroundService(HelloWorldService hws)
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