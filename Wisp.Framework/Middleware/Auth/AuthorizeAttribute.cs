namespace Wisp.Framework.Middleware.Auth;

public class AuthorizeAttribute(string? role = null) : Attribute
{
    public string? Role { get; init; } = role;
}