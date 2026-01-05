// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Microsoft.Extensions.Options;
using Wisp.Framework.Http;

namespace Wisp.Framework.Middleware;

public class CorsMiddleware(IOptions<CorsMiddlewareConfig> options) : HttpMiddleware
{
    private readonly CorsMiddlewareConfig _config = options.Value;
    
    public override Task OnRequestReceived(IHttpContext context)
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", _config.AccessControlAllowOrigin);

        if (_config.AccessControlExposeHeaders is not null)
            context.Response.Headers.Add("Access-Control-Expose-Headers", string.Join(", ", _config.AccessControlExposeHeaders));
        
        if(_config.AccessControlAllowHeaders is not null)
            context.Response.Headers.Add("Access-Control-Allow-Headers", string.Join(", ", _config.AccessControlAllowHeaders));
        
        if(_config.AccessControlAllowMethods is not null)
            context.Response.Headers.Add("Access-Control-Allow-Methods", string.Join(", ", _config.AccessControlAllowMethods));
        
        if(_config.AccessControlAllowCredentials is not null)
            context.Response.Headers.Add("Access-Control-Allow-Credentials", _config.AccessControlAllowCredentials.ToString() ?? "false");
        
        if(_config.AccessControlMaxAge is not null)
            context.Response.Headers.Add("Access-Control-Max-Age", _config.AccessControlMaxAge.ToString() ?? "0");

        return Task.CompletedTask;
    }
}