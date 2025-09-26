// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wisp.Framework.Configuration;
using Wisp.Framework.Http;
using Wisp.Framework.Http.Impl;
using Wisp.Framework.Http.Impl.NetCoreServer;
using Wisp.Framework.Middleware;
using Wisp.Framework.Middleware.Auth;
using Wisp.Framework.Middleware.ErrorPages;
using Wisp.Framework.Middleware.Sessions;
using Wisp.Framework.Util.ServiceDiscovery;
using Wisp.Framework.Views;

namespace Wisp.Framework;

public class WispHostBuilder
{

    private readonly IServiceCollection _serviceCollection = new ServiceCollection();
    
    public IServiceCollection Services => _serviceCollection;

    public IConfiguration Configuration => ConfigurationBuilder.Build();

    public readonly ConfigurationBuilder ConfigurationBuilder;

    private IServiceProvider? _serviceProvider;

    private readonly List<Action<IConfigurationBuilder>> _configBuilders = new();

    private readonly List<Action<IServiceCollection>> _serviceBuilders = new();
    
    private Action<ILoggingBuilder>? _loggingBuilderConfig;
    
    private Assembly? _serviceScannerAssembly;

    /// <summary>
    /// The host builder configures logging, configuration and dependency injection
    /// </summary>
    public WispHostBuilder()
    {
        ConfigurationBuilder = new ConfigurationBuilder();
        ConfigurationBuilder.AddJsonFile("wisp.json", optional: true);
        ConfigurationBuilder.AddJsonFile("wisp.development.json", optional: true);
    }

    // /// <summary>
    // /// Set up configuration.
    // /// </summary>
    // /// <param name="builder"></param>
    // /// <returns></returns>
    // public WispHostBuilder Configure(Action<IConfigurationBuilder> builder)
    // {
    //     _configBuilders.Add(builder);
    //
    //     return this;
    // }

    /// <summary>
    /// Configure dependency injection
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public WispHostBuilder ConfigureServices(Action<IServiceCollection> services)
    {
        _serviceBuilders.Add(services);

        return this;
    }

    /// <summary>
    /// Configure logging. This method should only be called once
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public WispHostBuilder ConfigureLogging(Action<ILoggingBuilder> builder)
    {
        _loggingBuilderConfig = builder;
        return this;
    }

    public WispHostBuilder UseBasicAuth(Action<BasicAuthConfigBuilder>? configBuilder = null)
    {
        var builder = new BasicAuthConfigBuilder();
        configBuilder?.Invoke(builder);
        _serviceCollection.AddSingleton<IAuthConfig, BasicAuthConfig>(_ => builder.Config);
        _serviceCollection.AddSingleton<IAuthenticator, BasicAuthenticator>();
        
        return this;
    }

    public WispHostBuilder UseFlashMessages()
    {
        _serviceCollection.AddSingleton<FlashService>();

        return this;
    }

    /// <summary>
    /// Enable the static file server middleware.
    /// </summary>
    /// <returns></returns>
    public WispHostBuilder UseStaticFiles()
    {
        _serviceCollection.AddSingleton<IHttpMiddleware, StaticFilesMiddleware>();
        return this;
    }

    /// <summary>
    /// A shortcut for adding middleware
    /// </summary>
    /// <param name="middleware"></param>
    /// <returns></returns>
    public WispHostBuilder AddMiddleware(Type middleware)
    {
        _serviceCollection.Add(new ServiceDescriptor(typeof(IHttpMiddleware), middleware, ServiceLifetime.Singleton));
        return this;
    }

    /// <summary>
    /// A shortcut for adding middleware
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public WispHostBuilder AddMiddleware<T>()
    {
        return AddMiddleware(typeof(T));
    }

    private bool _inMemorySessionEnabled = false;
    
    public WispHostBuilder UseInMemorySession()
    {
        if (_inMemorySessionEnabled) return this;
        
        _serviceCollection.AddSingleton<ISessionStore, InMemorySessionStore>();
        _serviceCollection.AddSingleton<IHttpMiddleware, SessionMiddleware>();
        _inMemorySessionEnabled = true;

        return this;
    }

    /// <summary>
    /// Enable the friendly error pages middleware. This will ensure that template-based HTML
    /// error pages are shown instead of JSON or plain-text ones.
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public WispHostBuilder UseFriendlyErrorPages(Action<ErrorPagesConfigBuilder>? config)
    {
        var cfg = new ErrorPagesConfigBuilder();
        config?.Invoke(cfg);

        Services.AddSingleton(cfg.Build());
        AddMiddleware<ErrorPageMiddleware>();

        return this;
    }

    public WispHostBuilder UseCors(IConfigurationSection? configSection = null)
    {
        configSection ??= Configuration.GetSection("Wisp:Extensions:Cors");
        Services.Configure<CorsMiddlewareConfig>(configSection);
        AddMiddleware<CorsMiddleware>();

        return this;
    }

    public WispHostBuilder UseServiceDiscovery(Assembly assembly)
    {
        _serviceScannerAssembly = assembly;
        return this;
    }

    /// <summary>
    /// Finalizes configuration and returns an application builder
    /// </summary>
    /// <returns></returns>
    public WispApplicationBuilder Build()
    {
        // _configBuilders.ForEach(c => c.Invoke(ConfigurationBuilder));
        var config = ConfigurationBuilder.Build();

        _serviceCollection.AddLogging(b =>
        {
            b.AddSimpleConsole();

            var logLevelConfig = config.GetSection("Wisp").GetSection("LogLevel").Get<string?>();
            b.SetMinimumLevel(Enum.TryParse(logLevelConfig, out LogLevel level) ? level : LogLevel.Information);

            if (_loggingBuilderConfig != null)
            {
                _loggingBuilderConfig.Invoke(b);
            }
        });

        _serviceCollection.AddSingleton<IConfiguration>(config);
        _serviceCollection.Configure<WispConfiguration>(config.GetSection("Wisp"));
        _serviceCollection.AddSingleton<Router>();
        _serviceCollection.AddSingleton<IHttpServer, NetCoreServerAdapter>();
        _serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        _serviceCollection.AddSingleton<TemplateRenderer>();
        _serviceCollection.AddSingleton<IMiddlewareDataInjector, MiddlewareDataInjector>();

        _serviceBuilders.ForEach(s => s.Invoke(_serviceCollection));

        if (_serviceScannerAssembly is not null)
        {
            var tempProvider = _serviceCollection.BuildServiceProvider();
            var logger = tempProvider.GetRequiredService<ILogger<ServiceRegistrar>>();
            var registrar = new ServiceRegistrar(_serviceCollection, logger);
            registrar.ScanAssembly(_serviceScannerAssembly);
        }

        _serviceProvider = _serviceCollection.BuildServiceProvider();

        var sessionProviders = _serviceProvider.GetServices<ISessionStore>().ToList();
        if (sessionProviders.Count() > 1)
        {
            throw new InvalidEnumArgumentException("more than one session store is configured, this is not allowed, please only use one");
        }

        return new WispApplicationBuilder(_serviceProvider);
    }
}
