using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wisp.Demo.Data;
using Wisp.Demo.Services;

using Wisp.Framework;

var hostBuilder = new WispHostBuilder();

hostBuilder.UseStaticFiles();

// Only one session store type can be used at the same time!
hostBuilder.UseInMemorySession();
//hostBuilder.AddRedisSessionStore();

hostBuilder.UseFlashMessages();
hostBuilder.UseBasicAuth(c => c.SetFailureRedirectUri("/unauthorized"));

hostBuilder.ConfigureServices(s =>
{
    s.AddDbContext<AppDbContext>(o =>
    {
        o.LogTo(Console.WriteLine, LogLevel.Debug);
        o.UseNpgsql("Server=localhost;Port=5432;Database=wispdemo;User Id=postgres;Password=wispadmin");
    });
    
    s.AddSingleton<AuthService>();
    s.AddSingleton<PostsService>();
});

var appBuilder = hostBuilder.Build();

//await appBuilder.UseRedisSessionStore();
appBuilder.UseControllers();

var app = appBuilder.Build();

await app.RunAsync();