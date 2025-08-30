using System.Linq.Expressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wisp.Extensions.Identity.OpenId.Data.Api;

namespace Wisp.Extensions.Identity.OpenId.Config;

public class OpenIdExtensionConfigBuilder(IServiceCollection serviceCollection)
{
    private readonly OpenIdExtensionConfig _config = new();
    
    public OpenIdExtensionConfigBuilder SetUsernameField(Expression<Func<OpenIdUserInfo, string?>> usernameProp)
    {
        _config.UsernameProp = usernameProp;

        return this;
    }

    public OpenIdExtensionConfigBuilder SetRolesClaimName(string name)
    {
        _config.RolesClaimName = name;
        
        return this;
    }

    public OpenIdExtensionConfigBuilder SetSuccessRedirectUri(string successRedirectUri)
    {
        _config.SuccessRedirectUri = successRedirectUri;
        return this;
    }

    public OpenIdExtensionConfigBuilder SetErrorRedirectUri(string errorRedirectUri)
    {
        _config.ErrorRedirectUri = errorRedirectUri;
        return this;
    }

    public OpenIdExtensionConfigBuilder SetAuthUrl(string url)
    {
        _config.AuthUrl = url;
        return this;
    }

    public OpenIdExtensionConfigBuilder SetCallbackUrl(string url)
    {
        _config.CallbackUrl = url;
        return this;
    }

    public OpenIdExtensionConfigBuilder SetLogoutUrl(string url)
    {
        _config.LogoutUrl = url;
        return this;
    }

    public OpenIdExtensionConfigBuilder SetDiscoveryUrl(string url)
    {
        _config.DiscoveryUrl = url;
        return this;
    }

    public OpenIdExtensionConfigBuilder SetClientId(string clientId)
    {
        _config.ClientId = clientId;
        return this;
    }

    public OpenIdExtensionConfigBuilder SetClientSecret(string clientSecret)
    {
        _config.ClientSecret = clientSecret;
        return this;
    }

    public OpenIdExtensionConfigBuilder SetScopes(string scopes)
    {
        _config.Scopes = scopes;
        return this;
    }
    
    public OpenIdExtensionConfig Build() => _config;

    public OpenIdExtensionConfigBuilder FromConfig(IConfigurationSection configSection)
    {
        configSection.Bind(_config);
        return this;
    }

}