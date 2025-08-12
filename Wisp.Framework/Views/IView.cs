// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Wisp.Framework.Http;

namespace Wisp.Framework.Views;

public interface IView
{ 
    string TemplateName { get; }
    
    bool IsRedirect { get; }
    
    string? RedirectUri { get; }
    
    object Model { get; }
}