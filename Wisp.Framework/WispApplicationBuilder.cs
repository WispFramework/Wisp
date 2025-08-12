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
using Wisp.Framework.Http;
using Wisp.Framework.Middleware.Auth;
using Wisp.Framework.Views;

namespace Wisp.Framework;

public class WispApplicationBuilder
{
    private readonly IServiceProvider _serviceProvider;

    public IServiceProvider Services => _serviceProvider;

    private readonly WispConfiguration _config;

    private readonly IHttpServer _server;
    
    private readonly Router _router;

    private readonly ILogger<WispApplicationBuilder> _log;

    /// <summary>
    /// The Application builder configures the Wisp application
    /// </summary>
    /// <param name="serviceProvider"></param>
    public WispApplicationBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _log = _serviceProvider.GetRequiredService<ILogger<WispApplicationBuilder>>();
        _config = _serviceProvider.GetRequiredService<IOptions<WispConfiguration>>().Value;
        
        _router = _serviceProvider.GetRequiredService<Router>();
        _server = _serviceProvider.GetRequiredService<IHttpServer>();
    }

    /// <summary>
    /// Enable controller discovery
    /// </summary>
    /// <returns></returns>
    public WispApplicationBuilder UseControllers()
    {
        var log = _serviceProvider.GetRequiredService<ILogger<ControllerRegistrar>>();
        var renderer = _serviceProvider.GetRequiredService<TemplateRenderer>();
        var auth = _serviceProvider.GetService<IAuthenticator>();
        ControllerRegistrar.RegisterControllers(_router, _serviceProvider, log, renderer, authenticator: auth);
        return this;
    }

    /// <summary>
    /// Manage route registrations
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public WispApplicationBuilder ConfigureRoutes(Action<Router> config)
    {
        config.Invoke(_router);
        return this;
    }

    /// <summary>
    /// Build the application
    /// </summary>
    /// <returns></returns>
    public WispApplication Build()
    {
        return new WispApplication(_serviceProvider, _server, _config);
    }
}