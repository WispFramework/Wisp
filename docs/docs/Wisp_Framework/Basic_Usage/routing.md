---
icon: lucide/route
---

# Routing

Wisp supports two routing models - anonymous function route handlers and class-based controllers.

For more info about controllers, see [Controllers](controllers).

## Anonymous Routes

For lightweight applications, Wisp supports defining routes using anonymous functions directly in your `Program.cs`.

!!! warning
    This feature is currently being reworked. This documentation section describes the new way of functioning that is
    currently only available in the `nightly` branch.

### Configuring Routes

Anonymous function based routes are added in the router configuration block.

```csharp
appBuilder.ConfigureRoutes(router => {
    // ...
});
```

For example, a Hello World GET handler would look something like this:

```csharp
appBuilder.ConfigureRoutes(router => {
    router.Get("/", ctx => new { Hello = "World" });
});
```

The above code will register a GET handler for `/` that returns the following JSON:

```json
{
    "Hello": "World"
}
```

The handler function can be synchronous or `async`, the `ctx` parameter is an instance of `IHttpContext` for the current
request. The expected return for the lambda is `object`. Any returned object will be serialized into JSON by default, unless
it's a `string` or a value type. Those will be returned to the client as is.

### HTTP Methods

Wisp supports all the current HTTP methods as outlined in [RFC 7231](https://datatracker.ietf.org/doc/html/rfc7231#autoid-33)
and the new [HTTP QUERY Method](https://www.ietf.org/archive/id/draft-ietf-httpbis-safe-method-w-body-05.html). The `QUERY`
method RFC is currently a draft and therefore the Wisp implementation of it may change if the RFC draft changes.

```csharp
router.Get()
router.Head()
router.Post()
router.Put()
router.Delete()
router.Connect()
router.Options()
router.Trace()
router.Query()
```

!!! warning
    **A note on dependency injection:** Every action handler receives an instance of `IHttpContext` which includes an instance of
    `IServiceProvider` (`ctx.Services`). This is the correct way of getting dependencies injected into your handler function. Unlike
    with class-based controllers, in anonymous handlers, you are responsible for maintaining the correct lifecycle of scoped dependencies.


    To correctly inject a scoped dependency, don't forget to create a scope first. And in `async` contexts, don't forget to use `await using`.
    
    ```csharp
    appBuilder.ConfigureRoutes(router => {
        // Synchronously
        router.Get("/", ctx => {
            using var scope = ctx.Services.CreateScope();
            var helloService = scope.GetRequiredService<HelloService>();
            return helloService.SayHelloWorld();
        });

        // Asynchronously
        router.Get("/items", async ctx => {
            await using var scope = ctx.Services.CreateAsyncScope();
            var service = scope.GetRequiredService<ItemService>();
            return await service.GetAllAsync();
        });
    });
    ```