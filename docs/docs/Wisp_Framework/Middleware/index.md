---
icon: lucide/link
---

# Middleware

Middlewares are services that run at specific stages of the request-response lifecycle.
They can read and manipulate both the request and the response.

## Middleware Stages

Middlewares can be called in multiple discrete stages in the Request-Response
lifecycle.

All middleware methods are asynchronous and may perform awaited operations.
Avoid blocking calls inside middleware.

Middleware stages execute in the following order:

1. `OnRequestReceived`
2. `OnRequestRouted`
3. Controller execution
4. `OnRequestHandled`

### `OnRequestReceived`

This method is called directly after the request has been received, before it
even reaches the router. Useful for Authentication, rejecting requests early
based on header values, etc.

### `OnRequestRouted`

This method is called after the router has determined which controller to call
but before calling the controller.

### `OnRequestHandled`

This method is called after the controller method has returned.

## Writing Custom Middleware

Custom middleware needs to implement the `IHttpMiddleware` interface. The
recommended way to create new middleware is to extend `HttpMiddleware`
rather than implementing the interface. That way, you can just override
those methods you care about and ignore the rest.

```csharp
public class MyMiddleware : HttpMiddleware 
{
    public override Task OnRequestReceived(IHttpContext context) 
    {
        if(context.Request.Headers.GetOrDefaultIgnoreCase("X-Reject-This-Request") == "true") 
        {
            context.Response.Body = new MemoryStream("Forbidden"u8);
            context.Response.StatusCode = 403;
            context.Response.IsHandled = true;
        }

        return Task.CompletedTask;
    }
}
```

If `context.Response.IsHandled` is set to `true`, the request pipeline stops and subsequent middleware
and the controller will not be executed.

If `IsHandled` is not set to `true`, execution continues to the next middleware stage.

## Registering Middleware

For middleware to be included in the request-response pipeline, you need to
register it.

There are two options, you can either use `WispHostBuilder#AddMiddleware<T>()`
or `IServiceCollection#Add(ServiceDescriptor)`.

Using the generic `WispHostBuilder` method is recommended.

```csharp
hostBuilder.Services.Add(new ServiceDescriptor(typeof(IHttpMiddleware), typeof(MyMiddleware), ServiceLifetime.Singleton));

// Recommended way:
hostBuilder.AddMiddleware<MyMiddleware>();
```

## Priority

By default, middlewares are executed in the order they're registered in. You
can override this order by setting a priority on the middleware. You can set
the middleware priority by implementing `MiddlewarePriority Priority { get; }`
(or overriding the `Priority` property of `HttpMiddleware`).

```csharp
public class MyMiddleware : HttpMiddleware 
{
    public override MiddlewarePriority Priority => MiddlewarePriority.Highest;
}
```

Setting the priority to highest will make this middleware always run first. If
you have more than one middleware with the same priority, they will be executed
in the order they're added.

MiddlewarePriority is not an enum! It's a struct that contains a few predefined
static values you can use.

You can also specify a custom numerical priority with `new MiddlewarePriority(int)`. 
Lower numeric values run earlier in the pipeline. `0` is the highest priority. 
Larger numbers run later.

!!! info
    **Pro Tip:** If you absolutely need your middleware to always run before
    anything else, you can set a negative priority (e.g. `-100_000`).

**Available pre-defined priorities are:**

- `Highest` = `0`
- `High` = `100`
- `Medium` = `1_000`
- `Low` = `10_000`
- `Lowest` = `100_000`