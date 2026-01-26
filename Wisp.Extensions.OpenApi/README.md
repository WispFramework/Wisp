# Wisp.Extensions.OpenApi

This extension adds an OpenAPI schema generator and a Swagger UI endpoint.

## Installation

Add the `Wisp.Extensions.OpenApi` NuGet package.

Enable the extension in your `WispHostBuilder`:

```csharp
hostBuilder.UseOpenApi(c => 
    c
      .Title("Demo")
      .Version("1.0.0")
      .InAssembly(Assembly.GetExecutingAssembly())
    
      .OpenApiSpecPath("/openapi/schema.json") // Optional
      .SwaggerUiPath("/openapi/swagger") // Optional
);
```

And map the appropriate endpoints in your `WispApplicationBuilder`:

```csharp
applicationBuilder.MapOpenApi();
```

## Usage

The extension will automatically scan for controllers annotated with `[ApiController]`. It will pick up
all routes in that controller with a valid `[Route]` attribute.

If a controller has a doc comment, the summary section will be shown in Swagger. Same goes for route
methods.

By default, the schema will be available at `/openapi/schema.json` and Swagger UI will be available
at `/openapi/swagger`.