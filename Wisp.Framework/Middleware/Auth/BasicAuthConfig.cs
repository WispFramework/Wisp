// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.
namespace Wisp.Framework.Middleware.Auth;

public class BasicAuthConfig : IAuthConfig
{
    public string FailureRedirectUri { get; set; } = "/";
}