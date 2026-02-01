// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Wisp.Framework.Controllers;
using Wisp.Framework.Middleware;
using Wisp.Framework.Middleware.ErrorPages;

namespace Wisp.Framework.Http;

/// <summary>
/// This is the Wisp HTTP Router
/// </summary>
public class Router(ILogger<Router> log, IEnumerable<IHttpMiddleware> middlewares)
{
    /// <summary>
    /// This is what a request handler method should conform to
    /// </summary>
    public delegate Task RequestHandler(IHttpContext request);

    private List<IHttpMiddleware> _middlewares = middlewares.ToList();

    private class RouteEntry
    {
        public Regex Pattern { get; set; }
        public RequestHandler Handler { get; set; }
        public int Priority { get; set; }
    }
    
    // private readonly Dictionary<string, Dictionary<Regex, RequestHandler>> Routes = new()
    // {
    //     {"GET", new Dictionary<Regex, RequestHandler>()},
    //     {"POST", new Dictionary<Regex, RequestHandler>()},
    //     {"PUT", new Dictionary<Regex, RequestHandler>()},
    //     {"PATCH", new Dictionary<Regex, RequestHandler>()},
    //     {"DELETE", new Dictionary<Regex, RequestHandler>()},
    //     {"OPTIONS", new Dictionary<Regex, RequestHandler>()},
    //     {"HEAD", new Dictionary<Regex, RequestHandler>()},
    //     {"QUERY", new Dictionary<Regex, RequestHandler>()}
    // };
    
    private readonly Dictionary<string, List<RouteEntry>> _routes = new()
    {
        {"GET", new List<RouteEntry>()},
        {"POST", new List<RouteEntry>()},
        {"PUT", new List<RouteEntry>()},
        {"PATCH", new List<RouteEntry>()},
        {"DELETE", new List<RouteEntry>()},
        {"OPTIONS", new List<RouteEntry>()},
        {"HEAD", new List<RouteEntry>()},
        {"QUERY", new List<RouteEntry>()}
    };

    /// <summary>
    /// Performs routing, selects the correct handler based on the route, runs it and returns the response.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Dispatch(IHttpContext context)
    {
        var request = context.Request;
        
        var method = request.Method;
        var uri = request.Path;

        if (string.IsNullOrWhiteSpace(method)) throw new Exception("the HTTP context does not contain a method");

        log.LogDebug("Trying to handle {Method} route for {Uri}", method, uri);

        if (_routes.TryGetValue(method, out var routes))
        {
            foreach (var route in routes.OrderByDescending(r => r.Priority))
            {
                var match = route.Pattern.Match(uri.TrimEnd('/').Split('?')[0]);
                if (match.Success)
                {
                    var routeParams = new Dictionary<string, string>();
                    foreach (var groupName in route.Pattern.GetGroupNames())
                    {
                        if (groupName != "0" && match.Groups[groupName].Success)
                            routeParams[groupName] = match.Groups[groupName].Value;
                    }

                    context.Request.PathVars = routeParams;

                    log.LogDebug("Found [{Method}] {Route}", method, uri);

                    foreach (var m in _middlewares.OrderBy(m => m.Priority.Value))
                    {
                        await m.OnRequestRouted();
                    }

                    await route.Handler.Invoke(context);
                    return;
                }
            }

            log.LogWarning("[{Method}] 404 Not Found - {Route}", method, uri);
            context.Response.StatusCode = 404;
            context.Response.Body = new MemoryStream("Not Found"u8.ToArray());
            context.ExtraData.Add(ErrorPageMiddleware.ExtraDataKey, new ErrorPageData { StatusCode = 404, FriendlyMessage = "Not Found", DeveloperMessage = $"no route found for [{method}] {uri}" });
            return;
        }

        log.LogWarning("[{Method}] 500 Unknown Method - {Route}", method, uri);

        context.Response.StatusCode = 500;
        context.Response.Body = new MemoryStream(Encoding.UTF8.GetBytes($"unknown method {method}"));
        context.ExtraData.Add(ErrorPageMiddleware.ExtraDataKey, new ErrorPageData { StatusCode = 404, FriendlyMessage = "Not Found", DeveloperMessage = $"unknown method [{method}] for {uri}" });
    }

    /// <summary>
    /// Clears the entire routing table.
    /// </summary>
    /// <remarks>This method mainly exists for internal use and should almost never be called from application code.</remarks>
    /// <returns></returns>
    internal Router Clear()
    {
        _routes.Clear();
        _routes["GET"] = new();
        _routes["POST"] = new();
        _routes["PUT"] = new();
        _routes["PATCH"] = new();
        _routes["DELETE"] = new();
        _routes["OPTIONS"] = new();
        _routes["HEAD"] = new();
        _routes["QUERY"] = new();

        return this;
    }

    public Router Add(RouteAttribute routeAttribute, RequestHandler handler, int priority = 0)
    {
        // _routes[routeAttribute.Method].Add(ConvertRouteTemplate(routeAttribute.Route), handler);
        _routes[routeAttribute.Method].Add(new RouteEntry
        {
            Pattern = ConvertRouteTemplate(routeAttribute.Route),
            Handler = handler,
            Priority = priority
        });
        return this;
    }

    /// <summary>
    /// Add a route handler
    /// </summary>
    /// <param name="method"></param>
    /// <param name="route"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Router Add(string method, string route, RequestHandler handler, int priority = 0)
    {
        //_routes[method].Add(ConvertRouteTemplate(route), handler);
        _routes[method].Add(new RouteEntry
        {
            Pattern = ConvertRouteTemplate(route),
            Handler = handler,
            Priority = priority
        });
        return this;
    }
    
    /// <summary>
    /// Add a GET route handler
    /// </summary>
    /// <param name="route"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Router Get(string route, RequestHandler handler, int priority = 0)
    {
        Add("GET", route, handler, priority);
        return this;
    }

    /// <summary>
    /// Add a POST route handler
    /// </summary>
    /// <param name="route"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Router Post(string route, RequestHandler handler, int priority = 0)
    {
        Add("POST", route, handler, priority);
        return this;
    }

    private Regex ConvertRouteTemplate(string template)
    {
        string pattern = Regex.Replace(template, @"\{([a-zA-Z0-9_]+)(?::([a-zA-Z0-9_\\*]+))?\}", match =>
        {
            var name = match.Groups[1].Value;
            var type = match.Groups[2].Success ? match.Groups[2].Value : "string";

            return type switch
            {
                "*" => $"(?<{name}>.+)",
                "int" => $"(?<{name}>\\d+)",
                "guid" => $"(?<{name}>[0-9a-fA-F\\-]{{36}})",
                "string" => $"(?<{name}>[^/]+)",
                _ => $"(?<{name}>[^/]+)"
            };
        });
        
        return new Regex($"^{pattern}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}