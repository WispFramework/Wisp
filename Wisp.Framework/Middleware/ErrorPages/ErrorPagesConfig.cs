// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Middleware.ErrorPages;

public class ErrorPagesConfig
{
    public string NotFoundTemplate { get; set; } = "/errors/404";

    public string UnauthorizedTemplate { get; set; } = "/error/401";

    public string ServerErrorTemplate { get; set; } = "/error/500";
}