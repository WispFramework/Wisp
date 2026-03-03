// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Wisp.Framework.Views;

namespace Wisp.Framework.Http;

/// <summary>
/// Middleware is a way of hooking into the request lifecycle at various points.
/// To add custom middleware:
///   - Implement <see cref="IHttpMiddleware"/>
///   - Add your implementation to DI
/// <remarks>It is recommended to implement <see cref="HttpMiddleware"/>, not IHttpMiddleware directly</remarks>
/// </summary>
public interface IHttpMiddleware
{
    /// <summary>
    /// This is called before the request has been processed
    /// </summary>
    /// <returns></returns>
    Task OnRequestReceived();

    /// <summary>
    /// This is called right after the request has been routed but before running the route code
    /// </summary>
    /// <returns></returns>
    Task OnRequestRouted();

    /// <summary>
    /// This is called after the request is fully processed but not sent yet
    /// </summary>
    /// <returns></returns>
    Task OnRequestHandled();

    /// <summary>
    /// This is called just before the template renders
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task OnTemplateRendering(ViewModel model);
    
    /// <summary>
    /// The priority, <see cref="MiddlewarePriority"/>
    /// </summary>
    MiddlewarePriority Priority { get; }
}