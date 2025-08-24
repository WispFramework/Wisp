namespace Wisp.Framework.Middleware.Auth;

public class BasicAuthConfig : IAuthConfig
{
    public string FailureRedirectUri { get; set; } = "/";
}