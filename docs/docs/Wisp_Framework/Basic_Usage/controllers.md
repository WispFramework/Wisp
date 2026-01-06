---
icon: lucide/monitor-cog
---
# Controllers

In Wisp, Controllers are classes that hold methods that are route handlers. A controller is a public,
non-static class that is marked with `[Controller]` or `[ApiController]` and optionally extends `ControllerBase`.

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

## Routing

Both the controller itself, and the methods within can have a `[Route]` attribute. The final route of a controller action
is determined by combining both the class-level and method-level `[Route]` attribute values.

For example:

```csharp
[ApiController]
[Route("/api/v1")]
public class HelloApiController : ControllerBase {
    [Route("hello-world")]
    public void ResultBox GetHello() {
        return Ok("Hello World");
    }
}
```

The final path to the `GetHello` controller action will be `/api/v1/hello-world`.

## Extending `ControllerBase`

Is is highly recommended, though not required, to extend the `ControllerBase` class. This will include some convenience methods
into your controller.

Here is a non-exhaustive overview of available method.

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

You can of course also create the ViewResults yourself.

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