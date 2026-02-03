# About

This extension adds a simple Admin Site generator that will generate CRUD
pages for any EFCore entities decorated with the `[GenerateCrud]` attribute.

!!! warning
    This extension is **very experimental**, reflection heavy, unoptimized and
    probably quite insecure. Use at your own risk.

## Installation

!!! warning
    The NuGet package is not available yet.

1. Install the NuGet Package
   ```shell
   dotnet add package Wisp.Extensions.Admin
   ```
2. Enable the admin generator
   ```csharp
   hostBuilder.AddAdmin(c => c.WithDbContextType(typeof(AppDbContext)));
   ```
3. Map the admin routes
   ```csharp
   appBuilder.MapAdminRoutes(Assembly.GetExecutingAssembly());
   ```
4. Run your app
   ```shell
   dotnet run
   ```
   and go to [http://localhost:6969/Admin](http://localhost:6969/Admin).
## Forms

The form generator can be customized using various [attributes](attributes).