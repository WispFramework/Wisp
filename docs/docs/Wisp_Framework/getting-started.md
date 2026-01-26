---
icon: lucide/package-open
---

# Getting Started

Starting a new Wisp project is super simple! You can either use a template or
create a new console app, install the Wisp package and do some basic setup yourself.

## A single-file micro API

This is the "Hello World" of modern frameworks. The smallest viable application that produces some output.

### Using the Templates

!!! warning
    The templates are not available yet. Coming Soon*(tm)*

```shell
dotnet new install Wisp.Framework.Templates

dotnet new wisp.api
```

### Manually

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

## A full web app with controllers and templates

This is an example of a more complete real-life application with controllers and templates.

### Using Templates

!!! warning
    The templates are not available yet. Coming Soon*(tm)*

```shell
dotnet new install Wisp.Framework.Templates

dotnet new wisp.mvc
```

### Manually

 1. Create a new Console application, add the Wisp dependency and create the basic folder structure.

    ```shell
    dotnet new console

    dotnet add package Wisp.Framework.Core

    mkdir -p ./{wwwroot|Templates|Controllers}
    ```

 2. Create a Wisp application in your `Program.cs` and enable basic functionality

    ```csharp
    var hostBuilder = new WispHostBuilder();
    
    // Enable the flash message middleware
    hostBuilder.UseFlashMessages();
    
    // Enable the in-memory session store
    // a session store is required for flash messages
    hostBuilder.UseInMemorySession();
    
    // Enable auto-discovery of services with the [Service] attribute
    hostBuilder.UseServiceDiscovery(Assembly.GetExecutingAssembly());
    
    // Enable serving static files from wwwroot/
    hostBuilder.UseStaticFiles();
    
    var appBuilder = hostBuilder.Build();
    
    // Enable controller autodiscovery
    appBuilder.UseControllers();
    
    var app = appBuilder.Build();
    
    await app.RunAsync();
    ```
 
 3. Add a controller to `Controllers/HelloWorldController.cs`
    ```csharp
    [Controller]
    public class HelloWorldController : ControllerBase
    {
        // Wisp strips the last '/' from URLs so the index route must match the empty string
        [Route("")]
        public ViewResult GetIndex() => View("index", new { Name = "World" });
    }   
    ```

 4. Create a CSS file in `wwwroot/app.css`
    ```css
    .hello-world {
        font-size: 4em;
        color: red;
    }
    ```

 5. Create a template in `Templates/index.liquid`
    ```html
    <!doctype html>
    <html lang="en">
    <head>
    <meta charset="UTF-8">
      <meta name="viewport" content="width=device-width, user-scalable=no, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0">
      <meta http-equiv="X-UA-Compatible" content="ie=edge">
      <title>Hello World</title>
    
      <!-- Link our stylesheet. Statis files from `wwwroot/` are available at `/` -->
      <link rel="stylesheet" href="/app.css" />
    </head>
    <body>
      <h1>Hello, {{ model.name }}!</h1>
    </body>
    </html>    
    ```

 6. Your project structure should now look like this:
    ```
    My.Project/
    ├─ Templates/
    │  ├─ index.liquid
    ├─ wwwroot/
    │  ├─ app.css
    ├─ Controllers/
    │  ├─ HelloWorldController.cs
    ├─ Program.cs
    ```

 7. Now, run your application with `dotnet run` and open [http://localhost:6969](http://localhost:6969) in
    your browser. If you did everything right, you should see `Hello, World!` in large red letters.

**Congratulations, you've created your first Wisp application!**