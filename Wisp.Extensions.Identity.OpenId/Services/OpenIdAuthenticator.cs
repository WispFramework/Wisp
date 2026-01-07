// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Wisp.Framework.Http;
using Wisp.Framework.Middleware.Auth;

namespace Wisp.Extensions.Identity.OpenId.Services;

public class OpenIdAuthenticator(IHttpContextAccessor contextAccessor) : IAuthenticator
{
    public const string OpenIdAuthenticatorPrincipalSessionKey = "oidc-principal";
    
    public Task<bool> AuthenticateRoute(List<string> roles)
    {
        var context = contextAccessor.HttpContext;
        if(context is null || context.Session is null) throw new Exception("session store not present");

        var principal = context.Session.Get<UserPrincipal>(OpenIdAuthenticatorPrincipalSessionKey);
        
        if (principal is null) return Task.FromResult(false);
        if (roles.Count < 1) return Task.FromResult(true);

        var rolesHash = new HashSet<string>(roles, StringComparer.OrdinalIgnoreCase);

        if (principal.Roles.Any(r => rolesHash.Contains(r))) return Task.FromResult(true);

        return Task.FromResult(false);
    }

    public Task<UserPrincipal?> GetUser()
    {
        var context = contextAccessor.HttpContext;
        if(context is null || context.Session is null) throw new Exception("session store not present");

        return Task.FromResult(context.Session.Get<UserPrincipal>(OpenIdAuthenticatorPrincipalSessionKey));
    }

    public Task<bool> Authenticate(UserPrincipal principal)
    {
        var context = contextAccessor.HttpContext;
        if(context is null || context.Session is null) throw new Exception("session store not present");

        context.Session.Set(OpenIdAuthenticatorPrincipalSessionKey, principal);
        
        return Task.FromResult(true);
    }

    public Task Deauthenticate()
    {
        var context = contextAccessor.HttpContext;
        if(context is null || context.Session is null) throw new Exception("session store not present");
        
        context.Session.Remove(OpenIdAuthenticatorPrincipalSessionKey);
        context.Session.Remove(OpenIdService.OpenIdStateSessionKey);
        context.Session.Remove(OpenIdService.OpenIdTokenSessionKey);
        return Task.CompletedTask;
    }
}