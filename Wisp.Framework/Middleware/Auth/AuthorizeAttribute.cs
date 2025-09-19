namespace Wisp.Framework.Middleware.Auth;

public class AuthorizeAttribute : Attribute
{
    public AuthorizeAttribute(string? role = null, string? authenticator = null)
    {
        if(role is not null) Roles.Add(role);
        Authenticator = authenticator;
    }

    public AuthorizeAttribute(string[] roles, string? authenticator = null)
    {
        Roles.AddRange(roles);
        Authenticator = authenticator;
    }

    public List<string> Roles { get; init; } = new();
    
    public string? Authenticator { get; init; }
}