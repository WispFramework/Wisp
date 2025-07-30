// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Controllers;

/// <summary>
/// Marks an HTTP route
/// </summary>
/// <param name="route"></param>
/// <param name="method"></param>
public class RouteAttribute(string route, string method = "GET") : Attribute
{
    public string Route { get; init; } = route;
    public string Method { get; init; } = method;
}