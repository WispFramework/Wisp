namespace Wisp.Framework.Middleware.Auth;

public class AuthorizeAttribute : Attribute
{
    public AuthorizeAttribute(string? role = null)
    {
        if(role is not null) Roles.Add(role);
    }

    public AuthorizeAttribute(string[] roles)
    {
        Roles.AddRange(roles);
    }

    public List<string> Roles { get; init; } = new();
}