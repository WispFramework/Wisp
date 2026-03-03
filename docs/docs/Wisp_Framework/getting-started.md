---
icon: lucide/package-open
---

# Getting Started

Starting a new Wisp project can be done using a template or manually.

## Using Templates

```shell
dotnet new install Wisp.Framework.Templates

dotnet new wisp.mvc

dotnet run
```

Then go to [http://localhost:6969](http://localhost:6969) and enjoy your new Wisp app.

*(Wisp defaults to the port `6969`)*

## Manually

 1. Create a new Console application, add the Wisp dependency and create the basic folder structure.

    ```shell
    dotnet new console

    dotnet add package Wisp.Framework.Core

    mkdir wwwroot
    mkdir Templates
    mkdir Controllers
    ```

 2. Create a Wisp application in your `Program.cs` and enable basic functionality

    ```csharp
    using System.Reflection;
    using Wisp.Framework;
    using Wisp.Framework.Controllers;

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
        // The root path "/" maps to an empty string route.
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
    
      <!-- Link our stylesheet. Static files from `wwwroot/` are available at `/` -->
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
    your browser. If everything is configured correctly, you should see `Hello, World!` rendered in red.

*(Wisp defaults to the port `6969`)*

**Congratulations, you've created your first Wisp application!**