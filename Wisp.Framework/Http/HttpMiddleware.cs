// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Wisp.Framework.Views;

namespace Wisp.Framework.Http;

public abstract class HttpMiddleware : IHttpMiddleware
{
    public virtual Task OnRequestHandled()
    {
        return Task.CompletedTask;
    }

    public virtual Task OnRequestReceived()
    {
        return Task.CompletedTask;
    }

    public virtual Task OnRequestRouted()
    {
        return Task.CompletedTask;
    }

    public virtual Task OnTemplateRendering(ViewModel model)
    {
        return Task.CompletedTask;
    }

    public virtual MiddlewarePriority Priority => MiddlewarePriority.Medium;
}