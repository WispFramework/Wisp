// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Wisp.Framework.Middleware.Sessions;

namespace Wisp.Framework.Views;

public class ViewModel
{
    public bool UserLoggedIn { get; set; } = false;

    public string CurrentUserName { get; set; } = string.Empty;

    public List<string> CurrentUserRoles { get; set; } = new();
    
    public string CurrentRoute { get; set; } = string.Empty;

    public List<FlashService.FlashMessage> FlashMessages { get; set; } = [];

    public required object Model { get; set; }

    public Dictionary<string, object?> Middleware { get; set; } = new();
    
    public string CurrentUserId { get; set; } = string.Empty;
}