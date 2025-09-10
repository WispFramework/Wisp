namespace Wisp.Framework.Middleware.Auth;

public class UserPrincipal
{
    public string Username { get; set; } = "";

    public List<string> Roles { get; set; } = new();

    public string Id { get; set; } = "";

    public string Email { get; set; } = "";

    public string FirstName { get; set; } = "";

    public string LastName { get; set; } = "";
}