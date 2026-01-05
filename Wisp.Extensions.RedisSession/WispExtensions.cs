// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Microsoft.Extensions.DependencyInjection;
using Wisp.Framework;
using Wisp.Framework.Http;
using Wisp.Framework.Middleware.Sessions;

namespace Wisp.Extensions.RedisSession;

public static class WispExtensions
{
    public static WispHostBuilder AddRedisSessionStore(this WispHostBuilder builder, Action<RedisSessionStoreBuilder>? configure = null)
    {
        var configBuilder = new RedisSessionStoreBuilder();
        
        configure?.Invoke(configBuilder);
        
        var config = configBuilder.Build();

        builder.Services.AddSingleton(config);
        builder.Services.AddSingleton<ISessionStore, RedisSessionStore>();
        builder.Services.AddSingleton<IHttpMiddleware, SessionMiddleware>();
        
        return builder;
    }

    public static async Task<WispApplicationBuilder> UseRedisSessionStore(this WispApplicationBuilder builder)
    {
        var provider = builder.Services.GetRequiredService<ISessionStore>();
        if (provider is RedisSessionStore s)
        {
            await s.InitializeAsync();
            return builder;
        }

        throw new ArgumentException("calling 'AddRedisSessionStore()' is required before calling 'UseRedisSessionStore()'");
    }
}