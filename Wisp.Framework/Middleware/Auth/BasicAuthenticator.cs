using Microsoft.Extensions.Logging;
using Wisp.Framework.Http;

namespace Wisp.Framework.Middleware.Auth;

public class BasicAuthenticator(IHttpContextAccessor accessor, ILogger<BasicAuthenticator> log, IAuthConfig config) : IAuthenticator
{
    private readonly List<UserPrincipal> _users = new();
    
    public async Task<bool> AuthenticateRoute(string? role = null)
    {
        var context = await accessor.HttpContext;
        if (context is null)
        {
            log.LogDebug("authentication failed: no context");
            return false;
        }
        
        var session = context.Session;
        if (session is null)
        {
            log.LogDebug("authentication failed: no session");
            return false;
        }

        var username = session.Get<string>("auth.username");
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
        
        if (role != null && user.Roles.Any(r => r.Equals(role, StringComparison.InvariantCultureIgnoreCase)))
        {
            log.LogDebug("authentication failed: role mismatch");
            return false;
        }

        return true;
    }

    public async Task<UserPrincipal?> GetUser()
    {
        var context = await accessor.HttpContext;
        if (context is null) return null;
        
        var session = context.Session;
        if(session is null) return null;
        
        var username = session.Get<string>("auth.username");
        if(username is null) return null;
        
        var user  = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));
        if(user == null) return null;
        
        return new UserPrincipal { Username = user.Username, Roles = user.Roles };
    }

    public async Task<bool> Authenticate(UserPrincipal principal)
    {
        var context = await accessor.HttpContext;
        if (context is null) return false;
        
        var session = context.Session;
        if(session is null) return false;
        
        session.Remove("auth.username");
        session.Set("auth.username", principal.Username);
        _users.Add(principal);
        
        return true;
    }

    public async Task Deauthenticate()
    {
        var context = await accessor.HttpContext;
        if (context is null) return;
        
        var session = context.Session;
        if (session is null) return;
        
        var user = session.Get<string>("auth.username");
        if(user is null) return;

        var principal = _users.FirstOrDefault(u => u.Username == user);
        if(principal is null) return;
        
        session.Remove("auth.username");
        _users.Remove(principal);
    }
}