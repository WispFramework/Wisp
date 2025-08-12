namespace Wisp.Framework.Middleware.Auth;

public interface IAuthConfig
{ 
    string FailureRedirectUri { get; set; }
}