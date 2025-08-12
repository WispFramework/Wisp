namespace Wisp.Framework.Middleware.Auth;

public class BasicAuthConfigBuilder
{
    private BasicAuthConfig _config = new();

    public BasicAuthConfigBuilder SetFailureRedirectUri(string redirectUri)
    {
        _config.FailureRedirectUri = redirectUri;
        return this;
    }

    public BasicAuthConfig Config => _config;
}