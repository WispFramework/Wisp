// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.
using Microsoft.Extensions.Logging;
using Wisp.Framework.Http;

namespace Wisp.Framework.Middleware.Auth;

public class BasicAuthenticator(IHttpContextAccessor accessor, ILogger<BasicAuthenticator> log, IAuthConfig config) : IAuthenticator
{
    private readonly List<UserPrincipal> _users = new();
    
    public Task<bool> AuthenticateRoute(List<string> roles)
    {
        var context = accessor.HttpContext;
        if (context is null)
        {
            log.LogDebug("authentication failed: no context");
            return Task.FromResult(false);
        }
        
        var session = context.Session;
        if (session is null)
        {
            log.LogDebug("authentication failed: no session");
            return Task.FromResult(false);
        }

        var username = session.Get<string>("auth.username");
        if (username is null)
        {
            log.LogDebug("authentication failed: no username in session");
            return Task.FromResult(false);
        }
        
        var user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));
        if (user == null)
        {
            log.LogDebug("authentication failed: username is invalid");
            return Task.FromResult(false);
        }

        var rolesHash = new HashSet<string>(roles, StringComparer.OrdinalIgnoreCase);

        if (roles.Count == 0 || !user.Roles.Any(r => rolesHash.Contains(r)))
        {
            log.LogDebug("authentication failed: role mismatch");
            log.LogDebug("user has roles: {Roles}", string.Join(", ", user.Roles));
            log.LogDebug("allowed roles: {Roles}", string.Join(", ", rolesHash));
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    public Task<UserPrincipal?> GetUser()
    {
        var context = accessor.HttpContext;
        if (context is null) return Task.FromResult<UserPrincipal?>(null);
        
        var session = context.Session;
        if(session is null) return Task.FromResult<UserPrincipal?>(null);
        
        var username = session.Get<string>("auth.username");
        if(username is null) return Task.FromResult<UserPrincipal?>(null);
        
        var user  = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));
        if(user == null) return Task.FromResult<UserPrincipal?>(null);
        
        return Task.FromResult<UserPrincipal?>(new UserPrincipal { Username = user.Username, Roles = user.Roles });
    }

    public Task<bool> Authenticate(UserPrincipal principal)
    {
        var context = accessor.HttpContext;
        if (context is null) return Task.FromResult(false);
        
        var session = context.Session;
        if(session is null) return Task.FromResult(false);
        
        session.Remove("auth.username");
        session.Set("auth.username", principal.Username);
        _users.Add(principal);
        
        return Task.FromResult(true);
    }

    public Task Deauthenticate()
    {
        var context = accessor.HttpContext;
        if (context is null) return Task.CompletedTask;
        
        var session = context.Session;
        if (session is null) return Task.CompletedTask;
        
        var user = session.Get<string>("auth.username");
        if(user is null) return Task.CompletedTask;

        var principal = _users.FirstOrDefault(u => u.Username == user);
        if(principal is null) return Task.CompletedTask;
        
        session.Remove("auth.username");
        _users.Remove(principal);
        return Task.CompletedTask;
    }
}