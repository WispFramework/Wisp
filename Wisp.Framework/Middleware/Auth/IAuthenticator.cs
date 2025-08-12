using Wisp.Framework.Http;

namespace Wisp.Framework.Middleware.Auth;

public interface IAuthenticator
{
    Task<bool> AuthenticateRoute(string? role = null);

    Task<UserPrincipal?> GetUser();

    Task<bool> Authenticate(UserPrincipal principal);
    
    Task Deauthenticate();
}