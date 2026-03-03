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
    router.Get("/", ctx => new ResultBox<object>(new { Hello = "World" }));
});
```

The above code will register a GET handler for `/` that returns the following JSON:

```json
{
    "Hello": "World"
}
```

The handler function can be synchronous or `async`, the `ctx` parameter is an instance of `IHttpContext` for the current
request. The expected return for the lambda is `IResultBox<>`.

Returned values are handled as follows:

 - value types like `string`, `int`, `bool`, etc. will be converted to their native string representation and sent in the response as `text/plain`
 - reference types will be serialized into JSON and sent in the response as `application/json`


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
            var helloService = scope.ServiceProvider.GetRequiredService<HelloService>();
            return new ResultBox(helloService.SayHelloWorld());
        });

        // Asynchronously
        router.Get("/items", async ctx => {
            await using var scope = ctx.Services.CreateAsyncScope();
            var service = scope.ServiceProvider.GetRequiredService<ItemService>();
            return await new ResultBox(service.GetAllAsync());
        });
    });
    ```

## Attribute Routing

For [controllers](controllers.md), it is usually more convenient to use attribute-based routing.

You can attach one or more `[Route]` attributes to a controller method to automatically register routes for that method.

```csharp
[Controller]
public class HelloController : ControllerBase 
{
    [Route("/")]
    [Route("/index")]
    public ViewResult GetIndex() 
    {
        return View("index");
    }
    
    [Route("/{slug}")]
    public ViewResult GetArticle(string slug, ArticleService articleService) 
    {
        var article = articleService.Get(slug);
        
        return View("article", new { Article = article });
    }
    
    [Route("/hello/{name}", "POST")]
    public ViewResult PostHello(string name, [FromForm] string message) 
    {
        return View("hello", new { Name = name, Message = message });
    }
}
```

The route attribute accepts two arguments - the path and the method.

 - The path is required.
 - The method argument is optional and defaults to `"GET"`
 - The method argument is a case-sensitive string. *This is a known pain point that needs to be addressed.
   Ideally, an enum should be used here.*

### Route Path Syntax

The route path consists of literal segments and optional variables. The basic
syntax for a path variable is `{variableName}`.

When injecting path variables as arguments, if the route matches but the input is not compatible with the target type,
Wisp will throw an exception and return a `500` error.

The available types are:

 - `string` - String (default)
 - `int` - integer
 - `guid` - Guid
 - *more will be added*

### Greedy Variables

This is a special variable type, denoted with a star (`{someVariable:*}`). A greedy variable can only exist at the end
of a route, and it consumes everything until the end, including slashes. Greedy variables **do not** affect query
parameters after a `?` in the URL.

For example:

`[Route("/api/v1/action/{subAction:*}")]` will match all of the following

 - `/api/v1/action/hello`
 - `/api/v1/action/hello/world`
 - `/api/v1/action/hello/world/foo`

It **will not**, however, match `/api/v1/action` or `/api/v1/action/` because the greedy segment must have some input.

### Multiple Routes

If you need your method to match multiple paths, allow multiple HTTP methods, or have an optional variable, you can specify
more than one `[Route]` attribute on it.

For example:

```csharp
[Route("/hello/world")]
[Route("/hello/world/{name}")]
public ViewResult Hello(string? name) {}
```

Will match both `/hello/world` and `/hello/world/my-name`.

```csharp
[Route("/hello/world")]
[Route("/hello/world", "POST")]
```

Will match both `GET` and `POST` HTTP methods