// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wisp.Framework.Configuration;
using Wisp.Framework.Controllers;
using Wisp.Framework.Hosting;
using Wisp.Framework.Http;
using Wisp.Framework.Middleware.Auth;
using Wisp.Framework.Views;

namespace Wisp.Framework;

public class WispApplication
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IHttpServer _server;

    private ILogger<WispApplication> _log;

    private readonly WispConfiguration _config;

    private readonly BackgroundServiceManager? _bsm;

    /// <summary>
    /// This is the main entrypoint
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="server"></param>
    /// <param name="config"></param>
    public WispApplication(IServiceProvider serviceProvider, IHttpServer server, WispConfiguration config)
    {
        _serviceProvider = serviceProvider;
        _server = server;
        _config = config;

        _bsm = serviceProvider.GetService<BackgroundServiceManager>();

        _log = _serviceProvider.GetRequiredService<ILogger<WispApplication>>();

        var featureFlags = _serviceProvider.GetRequiredService<IOptions<FeatureFlags>>().Value;
        
        #if DEBUG
        HotReloadHandler.UpdateApplicationEvent += types =>
        {
            _log.LogDebug("Hot-Reload detected");

            if (featureFlags.EnableControllerHotReload)
            {
                _log.LogDebug("Trying to re-register controllers...");
                ReRegisterControllers();
                _log.LogDebug("Hot-Reload completed");    
            }
        };
        #endif
    }

    private void ReRegisterControllers()
    {
        var log = _serviceProvider.GetRequiredService<ILogger<ControllerRegistrar>>();
        var renderer = _serviceProvider.GetRequiredService<TemplateRenderer>();
        var auth = _serviceProvider.GetService<IAuthenticator>();
        var router = _serviceProvider.GetRequiredService<Router>();
        ControllerRegistrar.RegisterControllers(router, _serviceProvider, log, renderer, authenticator: auth, clearPrevious: true);
    }

    /// <summary>
    /// Start the application
    /// </summary>
    /// <param name="cancel"></param>
    public async Task RunAsync(CancellationToken? cancel = default)
    {
        var realCancel = cancel ?? CancellationToken.None;

        var serverTask = _server.StartAsync(realCancel);
        var bsmTask = _bsm?.RunAsync(realCancel);

        _log.LogInformation("starting HTTP server on http://{Host}:{Port}/", _config.Host, _config.Port);
        _log.LogInformation("you can stop the server with CTRL+C");

        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            _log.LogInformation("SIGINT received, shutting down");

            Stop();

            Environment.Exit(0);
        };

        AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
        {
            _log.LogInformation("server shutting down...");
            Stop();
        };

        var voidTask = Task.Run(() => { }, realCancel);

        await Task.WhenAll(serverTask, bsmTask ?? voidTask, Task.Delay(-1, realCancel));
    }

    private void Stop()
    {
        var timeout = TimeSpan.FromSeconds(10);
        _bsm?.Stop();

        Task.Delay(timeout).ContinueWith(_ =>
        {
            _log.LogWarning("exceeded graceful shutdown timeout, shutting down");
            Environment.Exit(1);
        });
    }
}