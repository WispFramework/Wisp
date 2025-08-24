// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wisp.Framework.Extensions;
using Wisp.Framework.Http;
using Wisp.Framework.Middleware.Auth;
using Wisp.Framework.Middleware.Sessions;
using Wisp.Framework.Views;
using static Wisp.Framework.Utils;

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
                if (routeAttr == null) continue;

                var authAttr = method.GetCustomAttribute<AuthorizeAttribute>();

                Router.RequestHandler handler = async context =>
                {
                    if (!await AuthenticateAsync(context, authAttr, authenticator, authConfig, routeAttr, log, serviceProvider)) return;

                    var args = BuildControllerArgs(method, serviceProvider);
                    var result = await InvokeControllerAsync(method, controllerInstance, args);

                    EnsureResultBox(result, method, controllerInstance);

                    if (result is ViewResult viewResult)
                    {
                        await WriteViewResponseAsync(context, viewResult, renderer);
                    }
                    else
                    {
                        await WriteBoxResponseAsync(context, result!);
                    }
                };

                router.Add(routeAttr.Method, routeAttr.Route, handler);
            }
        }
    }

    private static object? GetDefault(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;

    private static async Task<bool> AuthenticateAsync(
        IHttpContext context,
        AuthorizeAttribute? authAttr,
        IAuthenticator? authenticator,
        IAuthConfig? authConfig,
        RouteAttribute routeAttr,
        ILogger log,
        IServiceProvider sp)
    {

        if (authAttr is null)
        {
            log.LogDebug("Not checking authentication for {Route} because it doesn't have an [Authorize] attribute", routeAttr.Route);
            return true;
        }

        if (authenticator is null)
        {
            log.LogDebug("Not checking authentication for {Route} because there's no IAuthenticator registered", routeAttr.Route);
            return true;
        }

        if (!await authenticator.AuthenticateRoute(authAttr.Role))
        {
            var flashService = sp.GetService<FlashService>();
            
            if (authConfig is not null)
            {
                if(flashService is not null) await flashService.AddFlashMessage("You are not authorized to access this resource", FlashService.FlashMessageType.Error);
                context.Response.StatusCode = 307;
                context.Response.Headers.Add("Location", authConfig.FailureRedirectUri);
            }
            else
            {
                context.Response.StatusCode = 401;
                context.Response.Body.Write("401 Unauthorized".AsUtf8Bytes());
            }

            return false;
        }

        return true;
    }

    private static object?[] BuildControllerArgs(MethodInfo method, IServiceProvider serviceProvider)
        => method.GetParameters()
            .Select(p => serviceProvider.GetService(p.ParameterType) ?? GetDefault(p.ParameterType))
            .ToArray();

    private static async Task<object?> InvokeControllerAsync(MethodInfo method, object controllerInstance, object?[] args)
    {
        var result = method.Invoke(controllerInstance, args)
            ?? throw new ArgumentException($"controller {controllerInstance.GetType().Name}#{method.Name} returned null");

        if (IsUnboundGenericInstance(result, typeof(Task<>)))
        {
            var task = (Task)result;
            await task;
            var resultProp = task.GetType().GetProperty("Result");
            return resultProp?.GetValue(task);
        }

        return result;
    }

    private static void EnsureResultBox(object? result, MethodInfo method, object controllerInstance)
    {
        if (result is null)
            throw new ArgumentException($"could not parse controller result for {controllerInstance.GetType().Name}#{method.Name}");

        if (!IsUnboundGenericInstance(result, typeof(IResultBox<>)))
            throw new ArgumentException($"controller {controllerInstance.GetType().Name}#{method.Name} did not return an implementation of IResultBox<>");
    }

    private static async Task WriteViewResponseAsync(IHttpContext context, ViewResult viewResult, TemplateRenderer renderer)
    {
        // TODO: [WISP-5] Custom Error Pages
        var view = viewResult.Value;

        if (view is null)
        {
            context.Response.StatusCode = 404;
            context.Response.Body.Write("Not Found".AsUtf8Bytes());
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
        context.Response.Body = new MemoryStream(content.AsUtf8Bytes());
    }

    private static async Task WriteBoxResponseAsync(IHttpContext context, object box)
    {
        var valueProp = box.GetType().GetProperty("Value");
        var value = valueProp?.GetValue(box)
            ?? throw new ArgumentException("the IResultBox<> value is null");

        var (serialized, isSimple) = ControllerResultSerializer.Serialize(value);

        context.Response.ContentType = isSimple ? "text/plain" : "application/json";
        context.Response.Body = new MemoryStream(serialized.AsUtf8Bytes());
    }
}