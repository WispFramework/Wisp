using Wisp.Framework.Http;
using Wisp.Framework.Middleware.Auth;

namespace Wisp.Extensions.Identity.OpenId.Services;

public class OpenIdAuthenticator(IHttpContextAccessor contextAccessor) : IAuthenticator
{
    public const string OpenIdAuthenticatorPrincipalSessionKey = "oidc-principal";
    
    public async Task<bool> AuthenticateRoute(string? role = null)
    {
        var context = await contextAccessor.HttpContext;
        if(context is null || context.Session is null) throw new Exception("session store not present");

        var principal = context.Session.Get<UserPrincipal>(OpenIdAuthenticatorPrincipalSessionKey);
        
        if (principal is null) return false;
        if (role is null) return true;
        if (principal.Roles.Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase))) return true;

        return false;
    }

    public async Task<UserPrincipal?> GetUser()
    {
        var context = await contextAccessor.HttpContext;
        if(context is null || context.Session is null) throw new Exception("session store not present");

        return context.Session.Get<UserPrincipal>(OpenIdAuthenticatorPrincipalSessionKey);
    }

    public async Task<bool> Authenticate(UserPrincipal principal)
    {
        var context = await contextAccessor.HttpContext;
        if(context is null || context.Session is null) throw new Exception("session store not present");

        context.Session.Set(OpenIdAuthenticatorPrincipalSessionKey, principal);
        
        return true;
    }

    public async Task Deauthenticate()
    {
        var context = await contextAccessor.HttpContext;
        if(context is null || context.Session is null) throw new Exception("session store not present");
        
        context.Session.Remove(OpenIdAuthenticatorPrincipalSessionKey);
        context.Session.Remove(OpenIdService.OpenIdStateSessionKey);
        context.Session.Remove(OpenIdService.OpenIdTokenSessionKey);
    }
}