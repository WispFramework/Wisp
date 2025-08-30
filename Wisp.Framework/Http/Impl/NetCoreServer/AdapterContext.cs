// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using NetCoreServer;
using Wisp.Framework.Middleware.Sessions;

namespace Wisp.Framework.Http.Impl.NetCoreServer;

/// <summary>
/// This is the NetCoreServer implementation of IHttpContext
/// </summary>
public class AdapterContext : IHttpContext
{
    private readonly AdapterResponse _response;

    public AdapterContext(HttpRequest req, HttpSession sess)
    {
        Request = new AdapterRequest(req);
        _response = new AdapterResponse(sess);
        Response = _response;

        IsHttps = false;
    }

    public bool IsHttps { get; private set; }

    public IHttpRequest Request { get; set; }

    public IHttpResponse Response { get; set; }
    
    public ISession? Session { get; set; }

    public bool IsHandled { get; set; }
}