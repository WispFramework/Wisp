// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.
using Microsoft.Extensions.Logging;
using Wisp.Framework.Http;
using Wisp.Framework.Middleware.Sessions;

namespace Wisp.Framework.Middleware.Auth;

public class BasicAuthenticator(IHttpContextAccessor accessor, ISessionAccessor sessionAccessor, ILogger<BasicAuthenticator> log, IAuthConfig config) : IAuthenticator
{
    private readonly List<UserPrincipal> _users = new();
    
    public async Task<bool> AuthenticateRoute(List<string> roles)
    {
        var username = await sessionAccessor.GetAsync<string>("auth.username");
        if (username is null)
        {
            log.LogDebug("authentication failed: no username in session");
            return false;
        }
        
        var user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));
        if (user == null)
        {
            log.LogDebug("authentication failed: username is invalid");
            return false;
        }

        var rolesHash = new HashSet<string>(roles, StringComparer.OrdinalIgnoreCase);

        if (roles.Count == 0 || !user.Roles.Any(r => rolesHash.Contains(r)))
        {
            log.LogDebug("authentication failed: role mismatch");
            log.LogDebug("user has roles: {Roles}", string.Join(", ", user.Roles));
            log.LogDebug("allowed roles: {Roles}", string.Join(", ", rolesHash));
            return false;
        }

        return true;
    }

    public async Task<UserPrincipal?> GetUser()
    {
        var username = await sessionAccessor.GetAsync<string>("auth.username");
        if(username is null) return null;
        
        var user  = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));

        return user;
    }

    public async Task<bool> Authenticate(UserPrincipal principal)
    {
        await sessionAccessor.ClearAsync("auth.username");
        await sessionAccessor.SetAsync("auth.username", principal.Username);
        
        _users.Add(principal);
        
        return true;
    }

    public async Task Deauthenticate()
    {
        var username = await sessionAccessor.GetAsync<string>("auth.username");
        if(username is null) return;

        var principal = _users.FirstOrDefault(u => u.Username == username);
        if(principal is null) return;
        
        await sessionAccessor.ClearAsync("auth.username");
        _users.Remove(principal);
    }
}