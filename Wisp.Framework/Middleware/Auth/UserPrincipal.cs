namespace Wisp.Framework.Middleware.Auth;

public class UserPrincipal
{
    public string Username { get; set; }
    
    public List<string> Roles { get; set; }
    
    public string Id { get; set; }
}