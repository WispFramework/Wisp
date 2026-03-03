using System.Reflection;
using Wisp.Framework;

var hostBuilder = new WispHostBuilder();

hostBuilder.UseInMemorySession();
hostBuilder.UseFlashMessages();
hostBuilder.UseServiceDiscovery(Assembly.GetExecutingAssembly());
hostBuilder.UseStaticFiles();

var appBuilder = hostBuilder.Build();

appBuilder.UseControllers();

var app = appBuilder.Build();

await app.RunAsync();