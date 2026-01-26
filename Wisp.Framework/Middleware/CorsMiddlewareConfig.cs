// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Middleware;

public class CorsMiddlewareConfig
{
    public string AccessControlAllowOrigin { get; set; } = "*";
    
    public List<string>? AccessControlExposeHeaders { get; set; }
    
    public int? AccessControlMaxAge { get; set; }
    
    public bool? AccessControlAllowCredentials { get; set; }
    
    public List<string>? AccessControlAllowMethods { get; set; }
    
    public List<string>? AccessControlAllowHeaders { get; set; }
}