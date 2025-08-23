using Wisp.Framework.Middleware.Sessions;

namespace Wisp.Framework.Views;

public class ViewModel
{
    public bool UserLoggedIn { get; set; } = false;

    public string CurrentUserName { get; set; } = string.Empty;

    public string CurrentUserRole { get; set; } = string.Empty;
    
    public string CurrentRoute { get; set; } = string.Empty;

    public List<FlashService.FlashMessage> FlashMessages { get; set; } = [];

    public required object Model { get; set; }

    public Dictionary<string, object?> Middleware { get; set; } = new();
}