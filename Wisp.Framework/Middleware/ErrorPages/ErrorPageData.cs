// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Middleware.ErrorPages;

public class ErrorPageData
{
    public int StatusCode { get; set; }

    public string FriendlyMessage { get; set; } = "";

    public string DeveloperMessage { get; set; } = "";

    public Exception? Exception { get; set; }
}