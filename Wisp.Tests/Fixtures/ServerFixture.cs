
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wisp.Framework;
using Wisp.Framework.Extensions;

namespace Wisp.Tests.Fixtures;

public class ServerFixture : IAsyncLifetime
{

    private CancellationTokenSource? _cts = new();

    public Task InitializeAsync()
    {
        var hostBuilder = new WispHostBuilder();

        hostBuilder.Configure(c => c.AddJsonFile("appsettings.Test.json", false));
        hostBuilder.ConfigureLogging(l => l.SetMinimumLevel(LogLevel.Debug));

        hostBuilder.UseStaticFiles();
        hostBuilder.UseFlashMessages();
        hostBuilder.UseBasicAuth(c => c.SetFailureRedirectUri("/unauthorized"));

        var appBuilder = hostBuilder.Build();

        appBuilder.UseControllers(Assembly.GetExecutingAssembly());
        appBuilder.ConfigureRoutes(r =>
        {
            r.Get("/", async ctx =>
            {
                ctx.Response.Body = new MemoryStream("Hello World".AsUtf8Bytes());
            });
        });

        var app = appBuilder.Build();

        _ = app.RunAsync(_cts?.Token);

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        if (_cts is not null) _cts.Cancel();
        return Task.CompletedTask;
    }
}