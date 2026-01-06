---
icon: lucide/package-open
---

# Getting Started

Starting a new Wisp project is super simple! You can either use a template or
create a new console app, install the Wisp package and do some basic setup yourself.

## Using the Templates

```shell
dotnet new install Wisp.Framework.Templates

dotnet new wisp.web
```

## Manually

Create a new project and install the Wisp NuGet package.

```shell
dotnet new console

dotnet add package Wisp.Framework.Core
```

Bootstrap a basic project.

```csharp title="Program.cs"
var hostBuilder = new new WispHostBuilder();

var appBuilder = hostBuilder.Build();

appBuilder.ConfigureRoutes(router => {
    router.Get("/", ctx => {
        ctx.Response.Body.Write("Hello World"u8);
    });
});

var app = appBuilder.Build();

await app.RunAsync();
```