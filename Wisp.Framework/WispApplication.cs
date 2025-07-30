// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wisp.Framework.Configuration;
using Wisp.Framework.Http;

namespace Wisp.Framework;

public class WispApplication
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IHttpServer _server;

    private ILogger<WispApplication> _log;

    private readonly WispConfiguration _config;

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

        _log = _serviceProvider.GetRequiredService<ILogger<WispApplication>>();
    }

    /// <summary>
    /// Start the application
    /// </summary>
    /// <param name="cancel"></param>
    public async Task RunAsync(CancellationToken? cancel = default)
    {
        var serverTask = _server.StartAsync();

        _log.LogInformation("starting HTTP server on http://{Host}:{Port}/", _config.Host, _config.Port);

        await Task.WhenAll(serverTask, Task.Delay(-1, cancel ?? CancellationToken.None));
    }
}