// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Views;

public interface IView
{ 
    bool IsRedirect { get; }
    Uri? RedirectUri { get; }
    
    object Model { get; }

    Task<(string HtmlOrAddress, bool IsRedirect)> Render();
}