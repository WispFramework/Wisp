using System.Linq;
using Microsoft.Extensions.Logging;
using Wisp.Framework.Http;

namespace Wisp.Framework.Middleware.Auth;

public class BasicAuthenticator(IHttpContextAccessor accessor, ILogger<BasicAuthenticator> log, IAuthConfig config) : IAuthenticator
{
    private readonly List<UserPrincipal> _users = new();
    
    public async Task<bool> AuthenticateRoute(List<string> roles)
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