// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Http;

public abstract class HttpMiddleware : IHttpMiddleware
{
    public virtual Task OnRequestHandled(IHttpContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnRequestReceived(IHttpContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnRequestRouted(IHttpContext context)
    {
        return Task.CompletedTask;
    }

    public virtual MiddlewarePriority Priority => MiddlewarePriority.Medium;
}