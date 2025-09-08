using Wisp.Framework.Http;

namespace Wisp.Framework.Middleware.Auth;

public interface IAuthenticator
{
    Task<bool> AuthenticateRoute(List<string> roles);

    Task<UserPrincipal?> GetUser();

    Task<bool> Authenticate(UserPrincipal principal);
    
    Task Deauthenticate();
}