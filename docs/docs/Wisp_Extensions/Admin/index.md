# About

This extension provides a simple admin site generator that automatically creates 
CRUD pages for EF Core entities decorated with the `[GenerateCrud]` attribute.

!!! warning
    This extension is **highly experimental**. It relies heavily on reflection,
    is not optimized for performance, and has not undergone security hardening.
    
    Do not expose it publicly without proper authentication and review.

## Installation

!!! warning
    The NuGet package is not yet published to nuget.org.
    For now, you must reference it locally or via a private feed.

1. Install the NuGet Package
   ```shell
   dotnet add package Wisp.Extensions.Admin
   ```
2. Enable the admin generator
   ```csharp
   // This tells the admin generator which EF Core `DbContext` to inspect for entities.
   hostBuilder.AddAdmin(c => c.WithDbContextType(typeof(AppDbContext)));
   ```
3. Map the admin routes
   ```csharp
   // This scans the specified assembly for entities marked with `[GenerateCrud]`
   // and registers the corresponding admin routes.
   appBuilder.MapAdminRoutes(Assembly.GetExecutingAssembly());
   ```
4. Run your app
   ```shell
   dotnet run
   ```
   and go to [http://localhost:6969/Admin](http://localhost:6969/Admin).
## Forms

The generated forms can be customized using additional [attributes](attributes)
to control field visibility, validation, and rendering behavior.