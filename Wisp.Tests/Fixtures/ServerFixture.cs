// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

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

        hostBuilder.ConfigurationBuilder.AddJsonFile("appsettings.Test.json", false);
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