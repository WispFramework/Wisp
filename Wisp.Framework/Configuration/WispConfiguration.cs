// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Configuration;

/// <summary>
/// Wisp configuration POCO
/// </summary>
public class WispConfiguration
{
    public required string Host { get; set; } = "0.0.0.0";

    public required int Port { get; set; } = 6969;

    public required string LogLevel { get; set; } = "Information";

    public required string StaticFileRoot { get; set; } = "wwwroot";
}