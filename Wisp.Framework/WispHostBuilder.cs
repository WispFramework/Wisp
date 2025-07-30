// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wisp.Framework.Configuration;
using Wisp.Framework.Http;
using Wisp.Framework.Http.Impl.NetCoreServer;
using Wisp.Framework.Middleware;

namespace Wisp.Framework;

public class WispHostBuilder
{

    private readonly IServiceCollection _serviceCollection = new ServiceCollection();

    private readonly ConfigurationBuilder _configBuilder;

    private IServiceProvider? _serviceProvider;

    private readonly List<Action<IConfigurationBuilder>> _configBuilders = new();

    private readonly List<Action<IServiceCollection>> _serviceBuilders = new();
    
    private Action<ILoggingBuilder>? _loggingBuilderConfig;

    /// <summary>
    /// The host builder configures logging, configuration and dependency injection
    /// </summary>
    public WispHostBuilder()
    {

        _configBuilder = new ConfigurationBuilder();
        _configBuilder.AddJsonFile("wisp.application.json", optional: false);
    }

    /// <summary>
    /// Set up configuration.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public WispHostBuilder Configure(Action<IConfigurationBuilder> builder)
    {
        _configBuilders.Add(builder);

        return this;
    }

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
    /// Finalizes configuration and returns an application builder
    /// </summary>
    /// <returns></returns>
    public WispApplicationBuilder Build()
    {
        _configBuilders.ForEach(c => c.Invoke(_configBuilder));
        var config = _configBuilder.Build();

        _serviceCollection.AddLogging(b =>
        {
            b.AddSimpleConsole();

            var logLevelConfig = config.GetRequiredSection("Wisp").GetRequiredSection("LogLevel").Get<string>();
            b.SetMinimumLevel(Enum.TryParse(logLevelConfig, out LogLevel level) ? level : LogLevel.Information);

            if (_loggingBuilderConfig != null)
            {
                _loggingBuilderConfig.Invoke(b);
            }
        });

        _serviceCollection.AddSingleton<IConfiguration>(config);
        _serviceCollection.Configure<WispConfiguration>(config.GetRequiredSection("Wisp"));
        _serviceCollection.AddSingleton<Router>();
        _serviceCollection.AddSingleton<IHttpServer, NetCoreServerAdapter>();

        _serviceBuilders.ForEach(s => s.Invoke(_serviceCollection));
        
        _serviceProvider = _serviceCollection.BuildServiceProvider();

        return new WispApplicationBuilder(_serviceProvider);
    }
}
