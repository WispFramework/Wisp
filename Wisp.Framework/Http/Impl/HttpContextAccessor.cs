// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Http.Impl;

public class HttpContextAccessor : IHttpContextAccessor
{

    private readonly AsyncLocal<HttpContextHolder> _currentContext = new();

    public Task SetContext(IHttpContext httpContext)
    {
        // _currentContext.Value = httpContext;
        if (_currentContext.Value != null)
        {
            _currentContext.Value.Context = httpContext;
        }
        else
        {
            _currentContext.Value = new HttpContextHolder { Context = httpContext };
        }
        
        return Task.CompletedTask;
    }

    public IHttpContext? HttpContext => _currentContext.Value?.Context;
}