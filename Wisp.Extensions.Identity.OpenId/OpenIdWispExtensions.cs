using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wisp.Extensions.Identity.OpenId.Config;
using Wisp.Extensions.Identity.OpenId.Data.Client;
using Wisp.Extensions.Identity.OpenId.Services;
using Wisp.Framework;
using Wisp.Framework.Middleware.Auth;
using Wisp.Framework.Middleware.Sessions;

namespace Wisp.Extensions.Identity.OpenId;

public static class OpenIdWispExtensions
{
    /// <summary>
    /// Enable the OpenID Auth Extension.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configBuilder"></param>
    /// <returns></returns>
    public static WispHostBuilder AddOpenIdConnect(this WispHostBuilder builder, Action<OpenIdExtensionConfigBuilder>? configBuilder = null)
    {
        builder.ConfigurationBuilder.AddJsonFile("wisp.extensions.identity.openid.json", optional: false);
        
        var cfgb = new OpenIdExtensionConfigBuilder(builder.Services);
        configBuilder?.Invoke(cfgb);
        builder.Services.AddSingleton(cfgb.Build());
        
        builder.Services.AddSingleton<OpenIdConnectClient>();
        builder.Services.AddSingleton<OpenIdService>();

        // Enable the default Session Store and IAuthenticator if none are present
        var tempServices = builder.Services.BuildServiceProvider();
        if(tempServices.GetService<ISessionStore>() is null)
            builder.UseInMemorySession();

        if (tempServices.GetService<IAuthenticator>() is null)
            builder.Services.AddSingleton<IAuthenticator, OpenIdAuthenticator>();
        
        return builder;
    }

    /// <summary>
    /// Map the OpenID Auth routes
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WispApplicationBuilder MapOpenIdConnect(this WispApplicationBuilder builder)
    {
        var config = builder.Services.GetRequiredService<OpenIdExtensionConfig>();
        var oidService = builder.Services.GetRequiredService<OpenIdService>();
        
        builder.ConfigureRoutes(r =>
        {
            r.Get(config.AuthUrl, oidService.GetAuthenticate);
            r.Get(config.CallbackUrl, oidService.GetAuthCallback);
            r.Get(config.LogoutUrl, oidService.GetLogout);
        });
        
        return builder;
    }
}