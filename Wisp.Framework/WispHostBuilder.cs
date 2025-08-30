// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wisp.Framework.Configuration;
using Wisp.Framework.Http;
using Wisp.Framework.Http.Impl;
using Wisp.Framework.Http.Impl.NetCoreServer;
using Wisp.Framework.Middleware;
using Wisp.Framework.Middleware.Auth;
using Wisp.Framework.Middleware.Sessions;
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
        
        _serviceProvider = _serviceCollection.BuildServiceProvider();

        var sessionProviders = _serviceProvider.GetServices<ISessionStore>().ToList();
        if (sessionProviders.Count() > 1)
        {
            throw new InvalidEnumArgumentException("more than one session store is configured, this is not allowed, please only use one");
        }

        return new WispApplicationBuilder(_serviceProvider);
    }
}
