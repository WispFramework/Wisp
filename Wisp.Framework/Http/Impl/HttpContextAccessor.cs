// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Http.Impl;

public class HttpContextAccessor : IHttpContextAccessor
{

    private readonly AsyncLocal<IHttpContext> _currentContext = new();

    public Task SetContext(IHttpContext httpContext)
    {
        _currentContext.Value = httpContext;
        
        return Task.CompletedTask;
    }

    public Task<IHttpContext?> HttpContext => Task.FromResult(_currentContext.Value);
}