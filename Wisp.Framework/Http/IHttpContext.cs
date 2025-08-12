// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Wisp.Framework.Middleware.Sessions;

namespace Wisp.Framework.Http;

/// <summary>
/// A wrapper for a single request lifecycle. Contains the request, the response and a handled flag for
/// middleware.
/// </summary>
public interface IHttpContext
{
    /// <summary>
    /// The request object
    /// </summary>
    IHttpRequest Request { get; }

    /// <summary>
    /// The response object
    /// </summary>
    IHttpResponse Response { get; }
    
    /// <summary>
    /// An optional session object
    /// </summary>
    ISession? Session { get; set; }

    /// <summary>
    /// If a context is handled, it shouldn't be processed any further and the response should be sent
    /// </summary>
    bool IsHandled { get; set; }
}