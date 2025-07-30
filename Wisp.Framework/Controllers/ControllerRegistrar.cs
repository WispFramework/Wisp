// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wisp.Framework.Http;
using Wisp.Framework.Views;

namespace Wisp.Framework.Controllers;

/// <summary>
/// The ControllerRegistrar performs controller discovery
/// </summary>
public class ControllerRegistrar
{
    public static void RegisterControllers(Router router, IServiceProvider serviceProvider, ILogger<ControllerRegistrar> log, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetEntryAssembly();

        var controllers = assembly?.GetTypes()
            .Where(t => t.GetCustomAttribute<ControllerAttribute>() != null) ?? throw new InvalidOperationException("this should not happen");

        foreach (var controllerType in controllers)
        {
            var controllerInstance = ActivatorUtilities.CreateInstance(serviceProvider, controllerType);

            foreach (var method in controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                var routeAttr = method.GetCustomAttribute<RouteAttribute>();
                if(routeAttr == null) continue;

                Router.RequestHandler handler = async context =>
                {
                    var parms = method.GetParameters();
                    var args = parms.Length == 0
                        ? Array.Empty<object>()
                        : new object[] { context };
                    
                    var result = method.Invoke(controllerInstance, args);
                    if (result is Task<IView> viewTask)
                    {
                        var view = await viewTask;
                        return await ViewToResponse(view);
                    }
                    else if (result is IView view)
                    {
                        return await ViewToResponse(view);
                    }
                    
                    throw new InvalidOperationException("Controller action did not return IView");
                };

                log.LogDebug("registering {Class}#{Method} as controller for [{Method}] {Route}",controllerType.FullName, method.Name, routeAttr.Method, routeAttr.Route);
                router.Add(routeAttr.Method, routeAttr.Route, handler);
            }
        }
    }

    private static async Task<IHttpResponse> ViewToResponse(IView view)
    {
        var (content, isRedirect) = await view.Render();
        var response = new WispHttpResponse
        {
            StatusCode = isRedirect ? 302 : 200,
            Body = new MemoryStream(Encoding.UTF8.GetBytes(content)),
            ContentType = "text/html",
        };

        if (isRedirect && view.RedirectUri != null)
        {
            response.Headers["Location"] = view.RedirectUri.ToString();
        }

        return response;
    }
}