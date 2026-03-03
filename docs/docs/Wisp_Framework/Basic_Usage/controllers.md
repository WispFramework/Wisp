---
icon: lucide/monitor-cog
---
# Controllers

Controllers in Wisp are classes that handle incoming HTTP requests and return responses in the form of views, JSON, or other data.
They follow a familiar MVC-style model.

## Controller Type Attribute

There are two basic controller types in Wisp. A `Controller` and an `ApiController`. Functionally, they are identical,
the only difference is that the OpenAPI Spec generator only looks at `ApiController`s and ignores regular `Controller`s.

```csharp
// This controller will not be included in the API spec
[Controller]
public class HelloController {}

[ApiController]
public class HelloApiController {}
```

### Priority

Normally, controller priority is determined on a last-in-first-out basis. That means that from two controllers
that contain the same route, the one added last will actually get to handle the request. Controller registration
order is not guaranteed when using automatic discovery. If two controllers define conflicting routes
(for example `/{a}/{b}` and `/about/info`), the more generic route may capture requests intended for the specific one.

You can explicitly specify a priority for a controller by setting the `priority` argument of `[Controller]`. Controllers
with a higher priority will be registered last and therefore take precedence. The default priority is `0`, which means
controllers without an explicitly set priority will compete for routes as usual. (In practice, this happens in alphabetical
order, but this is not guaranteed)

```csharp
[Controller]
public class GenericController 
{
    [Route("/{a}/{b}")]
    public ViewResult GetSomething(string a, string b) => View("");
}

[Controller(priority: 1_000)]
public class SpecificController
{
    [Route("/hello/world")]
    public ViewResult GetSomething() => View("");
}
```

In the example above, the handler for `/{a}/{b}` will not consume `/hello/world`, because the priority of `SpecificController`
is higher. Routes matching `/{a}/{b}` but not `/hello/world` will still be correctly consumed by `GenericController`.

!!! info
    **Pro Tip:** You can also make the priority negative to make sure a controller gets lower priority than others. 

    This is useful when you have many specific controllers and one generic controller that conflicts with them. This way,
    you only need to set the priority once.

    For example: `[Controller(priority: -1_000)]`

## Routing

The final route of a controller action is determined by combining both the class-level and method-level [Route] attribute values.

If the method-level route starts with `/`, it is treated as an absolute path and does not combine with the class-level route.

For example:

```csharp
[ApiController]
[Route("/api/v1")]
public class HelloApiController : ControllerBase {
    [Route("hello-world")]
    public void IResultBox<string> GetHello() {
        return Ok("Hello World");
    }
}
```

The final path to the `GetHello` controller action will be `/api/v1/hello-world`.

## Extending `ControllerBase`

It is highly recommended, though not required, to extend the `ControllerBase` class. This will include some convenience methods
into your controller.

Here is a non-exhaustive overview of available methods:

```csharp

/// Creates an instance of ViewResult
View(string templateName, object? model)

/// Creates an instance of ViewResult with a redirect
Redirect(string url);

/// Creates a generic instance of IResultBox around `item`
Box<T>(T item);

/// Extracts information from an `Error` object and builds a response out of it
FromError(Error error);

/// Creates an error response and returns it with a 500 code
ServerError(string message, string? description = null, Exception? exception = null)

/// Creates an error response with a 404 error code
NotFound(string message, string? description = null, Exception? exception = null)

/// Creates an instance of `IResultBox` around `content` with a 200 response code
Ok<T>(T content)

```

## Views

In a user-facing non-API application, you will generally want controller actions to return
some HTML. This is done using Views. Wisp follows the `Controller-Model-Template` model.

The controller is the bridge that invokes services with business logic and produces a
model object that is then passed to a template.

Wisp uses the Fluid templating engine (which is a C# implementation of Shopify Liquid).
See [Templating](templating) for more information.

A View Controller returns a `ViewResult` that tells the templating engine what to send
to the user. A `ViewResult` can either point to a template and optionally contain data
for the template (the Model) or it can be a redirect.

`ControllerBase` contains convenience methods for creating ViewResult objects.

```csharp
[Controller]
public class HomeController : ControllerBase {
    [HttpGet]
    [Route("/")]
    public ViewResult GetIndex() => View("index", new { Name = "John Doe" });

    [HttpGet]
    [Route("/go-home")]
    public ViewResult GoHome() => Redirect("/");
}
```

You can also construct `ViewResult` instances manually if you prefer explicit control.

```csharp
[Controller]
public class HomeController {
    [HttpGet]
    [Route("/")]
    public ViewResult GetIndex() 
      => new ViewResult(new TemplateView("index", new { Name = "John Doe" }));

    [HttpGet]
    [Route("/go-home")]
    public ViewResult GoHome()
      => new ViewResult(new RedirectView("/"));
}
```

## Hot-Reload

When running with `dotnet watch`, Wisp will try to detect hot reloads and re-run controller registration.

Hot reload re-runs controller discovery and route registration, but does not rebuild the DI container.

**This feature is experimental, but enabled by default.** You can disable it by setting `EnableControllerHotReload` in
the `FeatureFlags` config section to `false`.

```json
"FeatureFlags": {
  "EnableControllerHotReload": true
}
```

### Supported Edits

Since this functionality completely re-runs the controller discovery, the behavior should be almost the same as restarting
the application, with some caveats.

The following edits should generally be expected to work correctly:

 - Controller Action Method Added
 - Controller Action Method Changed
 - Controller Action Method Removed
 - Route Attribute Added
 - Route Attribute Changed
 - Route Attribute Removed
 - Controller Class Added
 - Controller Class Changed
 - Controller Class Removed
 - Controller Class Constructor Changed

**What will not work are any changes to the DI container (adding/removing services), configuration or middleware. See
[Wisp Architecture](../advanced/architecture#hot-reload) for more details.**