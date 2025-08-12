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
using Wisp.Framework.Middleware.Auth;
using Wisp.Framework.Views;

namespace Wisp.Framework.Controllers;

/// <summary>
/// The ControllerRegistrar performs controller discovery
/// </summary>
public class ControllerRegistrar
{
    public static void RegisterControllers(
        Router router, 
        IServiceProvider serviceProvider, 
        ILogger<ControllerRegistrar> log,
        TemplateRenderer renderer,
        IAuthenticator? authenticator = null,
        Assembly? assembly = null)
    {
        assembly ??= Assembly.GetEntryAssembly();

        var authConfig = serviceProvider.GetService<IAuthConfig>();

        var controllers = assembly?.GetTypes()
            .Where(t => t.GetCustomAttribute<ControllerAttribute>() != null) ?? throw new InvalidOperationException("this should not happen");

        foreach (var controllerType in controllers)
        {
            
            var controllerInstance = ActivatorUtilities.CreateInstance(serviceProvider, controllerType);

            foreach (var method in controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                var routeAttr = method.GetCustomAttribute<RouteAttribute>();
                if(routeAttr == null) continue;

                var authAttr = method.GetCustomAttribute<AuthorizeAttribute>();

                Router.RequestHandler handler = async context =>
                {
                    if (authAttr is not null && authenticator is not null)
                    {
                        var isAuthOk = await authenticator.AuthenticateRoute(authAttr.Role);

                        if (!isAuthOk)
                        {
                            if (authConfig != null)
                            {
                                context.Response.StatusCode = 307;
                                context.Response.Headers.Add("Location", authConfig.FailureRedirectUri);
                            }
                            else
                            {
                                context.Response.StatusCode = 401;
                                context.Response.Body.Write(Encoding.UTF8.GetBytes("Unauthorized"));    
                            }
                            
                            return;
                        }
                    }
                    else
                        log.LogDebug("not checking authentication for {Route} because it doesn't have an [Authorize] attribute or there is no authenticator registered", routeAttr.Route);
                    
                    var parms = method.GetParameters();
                    var args = parms
                        .Select(p => serviceProvider.GetService(p.ParameterType) ?? GetDefault(p.ParameterType))
                        .ToArray();
                    
                    var result = method.Invoke(controllerInstance, args);
                    
                    IView? view = null;
                    if (result is Task<IView> viewTask)
                    {
                        view = await viewTask;
                    }
                    else if (result is IView v)
                    {
                        view = v;
                    }

                    if (view is null)
                    {
                        context.Response.StatusCode = 404;
                        context.Response.Body = new MemoryStream("Not Found"u8.ToArray());
                        return;
                    }

                    if (view.IsRedirect)
                    {
                        context.Response.StatusCode = 302;
                        context.Response.Headers["Location"] = view.RedirectUri ?? "/";
                        return;
                    }

                    var content = await renderer.Render(view.TemplateName, view.Model, context);
                    
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "text/html";
                    context.Response.Body = new MemoryStream(Encoding.UTF8.GetBytes(content));
                };
                
                router.Add(routeAttr.Method, routeAttr.Route, handler);
            }
        }
    }
    
    private static object? GetDefault(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;
}