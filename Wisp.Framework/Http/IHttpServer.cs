// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Http;

/// <summary>
/// A wrapper for an HTTP server
/// </summary>
public interface IHttpServer
{
    /// <summary>
    /// Start the server
    /// </summary>
    /// <returns></returns>
    Task StartAsync();
    
    /// <summary>
    /// Stop the server
    /// </summary>
    /// <returns></returns>
    Task StopAsync();
}