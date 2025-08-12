using Microsoft.Extensions.Logging;
using Wisp.Demo.Data;
using Wisp.Demo.Data.Models;
using Wisp.Framework.Middleware.Auth;

namespace Wisp.Demo.Services;

public class AuthService(AppDbContext db, IAuthenticator authenticator, ILogger<AuthService> log)
{
    public async Task<bool> Login(string username, string password)
    {
        var user = db.Users.FirstOrDefault(u => u.Username == username.ToLowerInvariant());
        if (user is null) return false;
        
        if (BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            await authenticator.Authenticate(new UserPrincipal { Username = user.Username, Role = user.Role });
            return true;
        }

        return false;
    }

    public async Task<bool> Register(string username, string password)
    {
        var existingUser = db.Users.Any(u => u.Username == username.ToLowerInvariant());
        if (existingUser) return false;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        
        var user = new User() { Username = username.ToLowerInvariant(), Password = passwordHash, Role = "user" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        return true;
    }

    public async Task Logout()
    {
        await authenticator.Deauthenticate();
    }
}