# Basic Usage

To include OpenAPI documentation and the Swagger UI in your Wisp project,
you will need to install the NuGet package and add some basic configuration.

## Install the NuGet Package

```shell
dotnet add package Wisp.OpenAPI
```

## Configure the Extension

```csharp title="Program.cs"
var hostBuilder = new WispHostBuilder();

hostBuilder.UseOpenApi(); // (1)!

var appBuilder = hostBuilder.Build();

appBuilder.MapOpenApi(); // (2)!

var app = appBuilder.Build();
await app.RunAsync();
```

1. Enable the OpenAPI Extensions
2. Map OpenAPI Endpoints for the schema JSON and for Swagger UI.

## Access Swagger UI

By default, swagger UI will be available at `/openapi/swagger` and
the schema will be at `/openapi/schema.json`.