// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Wisp.Framework.Middleware.Auth;
using Wisp.Framework.Middleware.Sessions;

namespace Wisp.Extensions.Identity.OpenId.Services;

public class OpenIdAuthenticator(ISessionAccessor sessionAccessor) : IAuthenticator
{
    public const string OpenIdAuthenticatorPrincipalSessionKey = "oidc-principal";
    
    public async Task<bool> AuthenticateRoute(List<string> roles)
    {
        var principal = await sessionAccessor.GetAsync<UserPrincipal>(OpenIdAuthenticatorPrincipalSessionKey);
        
        if (principal is null) return false;
        if (roles.Count < 1) return true;

        var rolesHash = new HashSet<string>(roles, StringComparer.OrdinalIgnoreCase);

        return principal.Roles.Any(r => rolesHash.Contains(r));
    }

    public async Task<UserPrincipal?> GetUser()
    {
        return await sessionAccessor.GetAsync<UserPrincipal>(OpenIdAuthenticatorPrincipalSessionKey);
    }

    public async Task<bool> Authenticate(UserPrincipal principal)
    {
        await sessionAccessor.SetAsync(OpenIdAuthenticatorPrincipalSessionKey, principal);
        return true;
    }

    public async Task Deauthenticate()
    {
        await sessionAccessor.ClearAsync(OpenIdAuthenticatorPrincipalSessionKey);
        await sessionAccessor.ClearAsync(OpenIdService.OpenIdStateSessionKey);
        await sessionAccessor.ClearAsync(OpenIdService.OpenIdTokenSessionKey);
    }
}