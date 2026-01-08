// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.
using Microsoft.Extensions.Logging;
using Wisp.Framework.Middleware.Sessions;

namespace Wisp.Framework.Middleware.Auth;

public class BasicAuthenticator(ISessionAccessor sessionAccessor, ILogger<BasicAuthenticator> log, IAuthConfig config) : IAuthenticator
{
    public async Task<bool> AuthenticateRoute(List<string> roles)
    {
        var principal = await sessionAccessor.GetAsync<UserPrincipal>("auth.principal");
        if (principal is null)
        {
            log.LogDebug("No user principal in session");
            return false;
        }
        
        var rolesHash = new HashSet<string>(roles, StringComparer.OrdinalIgnoreCase);

        if (roles.Count == 0 || !principal.Roles.Any(r => rolesHash.Contains(r)))
        {
            log.LogDebug("authentication failed: role mismatch");
            log.LogDebug("user has roles: {Roles}", string.Join(", ", principal.Roles));
            log.LogDebug("allowed roles: {Roles}", string.Join(", ", rolesHash));
            return false;
        }

        return true;
    }

    public async Task<UserPrincipal?> GetUser()
    {
        var principal = await sessionAccessor.GetAsync<UserPrincipal>("auth.principal");

        return principal;
    }

    public async Task<bool> Authenticate(UserPrincipal principal)
    {
        await sessionAccessor.ClearAsync("auth.principal");
        await sessionAccessor.SetAsync("auth.principal", principal);
        
        return true;
    }

    public async Task Deauthenticate()
    {

        var principal = await sessionAccessor.GetAsync<UserPrincipal>("auth.principal");
        if(principal is null) return;
        await sessionAccessor.ClearAsync("auth.principal");
    }
}