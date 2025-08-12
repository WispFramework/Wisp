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

namespace Wisp.Framework.Http;

/// <summary>
/// This is the Wisp HTTP Router
/// </summary>
public class Router(ILogger<Router> log)
{
    /// <summary>
    /// This is what a request handler method should conform to
    /// </summary>
    public delegate Task RequestHandler(IHttpContext request);

    private readonly Dictionary<string, Dictionary<Regex, RequestHandler>> Routes = new()
    {
        {"GET", new Dictionary<Regex, RequestHandler>()},
        {"POST", new Dictionary<Regex, RequestHandler>()},
        {"PUT", new Dictionary<Regex, RequestHandler>()},
        {"PATCH", new Dictionary<Regex, RequestHandler>()},
        {"DELETE", new Dictionary<Regex, RequestHandler>()},
        {"OPTIONS", new Dictionary<Regex, RequestHandler>()},
        {"HEAD", new Dictionary<Regex, RequestHandler>()},
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

        if (string.IsNullOrWhiteSpace(method)) throw new Exception("the HTTP context has a fucked up method");

        log.LogDebug("Trying to handle {Method} route for {Uri}", method, uri);
        
        if (Routes.TryGetValue(method, out var routes))
        {
            foreach (var route in routes)
            {
                var match = route.Key.Match(uri.Split('?')[0]);
                if (match.Success)
                {
                    var routeParams = new Dictionary<string, string>();
                    foreach (var groupName in route.Key.GetGroupNames())
                    {
                        if(groupName != "0" && match.Groups[groupName].Success)
                            routeParams[groupName] = match.Groups[groupName].Value;
                    }
                    
                    context.Request.PathVars = routeParams;
                    
                    log.LogDebug("Found [{Method}] {Route}", method, uri);
                    await route.Value.Invoke(context);
                    return;
                }
            }
            
            log.LogWarning("[{Method}] 404 Not Found - {Route}", method, uri);
            context.Response.StatusCode = 404;
            context.Response.Body = new MemoryStream("Not Found"u8.ToArray());
            return;
        }

        log.LogWarning("[{Method}] 500 Unknown Method - {Route}", method, uri);

        context.Response.StatusCode = 500;
        context.Response.Body = new MemoryStream(Encoding.UTF8.GetBytes($"unknown method {method}"));
    }

    /// <summary>
    /// Add a route handler
    /// </summary>
    /// <param name="method"></param>
    /// <param name="route"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Router Add(string method, string route, RequestHandler handler)
    {
        Routes[method].Add(ConvertRouteTemplate(route), handler);
        return this;
    }
    
    /// <summary>
    /// Add a GET route handler
    /// </summary>
    /// <param name="route"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Router Get(string route, RequestHandler handler)
    {
        Routes["GET"].Add(ConvertRouteTemplate(route), handler);
        return this;
    }

    /// <summary>
    /// Add a POST route handler
    /// </summary>
    /// <param name="route"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Router Post(string route, RequestHandler handler)
    {
        Routes["POST"].Add(ConvertRouteTemplate(route), handler);
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