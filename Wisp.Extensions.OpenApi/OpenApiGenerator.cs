// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using Namotion.Reflection;
using Wisp.Extensions.OpenApi.Config;
using Wisp.Framework.Controllers;

namespace Wisp.Extensions.OpenApi;

public class OpenApiGenerator(OpenApiConfig config, ILogger<OpenApiGenerator> log)
{
    private string? _specJson;
    
    public string GetSpecJson()
    {
        return _specJson ??= Generate();
    }
    
    public string Generate()
    {
        var assembly = config.Assembly;
        if(assembly is null) return string.Empty;

        var doc = new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Title = config.Title,
                Version = config.Version,
            },
            Servers = [new OpenApiServer { Url = "http://localhost:6969" }],
            Paths = new OpenApiPaths(),
            Tags = new HashSet<OpenApiTag>()
        };
        
        log.LogInformation("scanning for controllers in {AssemblyName}", assembly.FullName);

        var controllerTypes = assembly.GetTypes()
            .Where(t => 
                t.GetCustomAttribute<ControllerAttribute>() != null && 
                t.GetCustomAttribute<ApiControllerAttribute>() != null)
            .ToList();

        foreach (var controller in controllerTypes)
        {
            log.LogInformation("scanning for controller methods in {ControllerName}", controller.FullName);

            var controllerSummary = controller.GetXmlDocsSummary();
            doc.Tags.Add(new OpenApiTag
            {
                Name = controller.Name,
                Description = (string.IsNullOrEmpty(controllerSummary))? $"Endpoints for the {controller.Name} controller" : controllerSummary
            });
            
            var controllerMethods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => m.GetCustomAttribute<RouteAttribute>() != null)
                .ToList();

            foreach (var method in controllerMethods)
            {
                log.LogInformation("scanning method {MethodName}", method.Name);
                
                var routeAttr = method.GetCustomAttribute<RouteAttribute>();
                if(routeAttr is null) continue;
                
                var rawPath = routeAttr.Route;
                var routeMethod = routeAttr.Method;
                var summary = method.GetXmlDocsSummary();
                
                log.LogInformation("{Method} route for path {Path} -> {MethodName}", routeMethod, rawPath, method.Name);

                var paramMatches = Regex.Matches(rawPath, @"\{([^\}]+)\}");
                var pathParameters = new List<IOpenApiParameter>();

                foreach (Match match in paramMatches)
                {
                    var paramName = match.Groups[1].Value.Split(':')[0];
                    pathParameters.Add(new OpenApiParameter
                    {
                        Name = paramName,
                        In = ParameterLocation.Path,
                        Required = true,
                        Schema = new OpenApiSchema { Type = JsonSchemaType.String }
                    });
                }

                var normalizedPath = Regex.Replace(rawPath, @"\{([^\}:]+)(:[^\}]+)?\}", @"{$1}");

                if (!doc.Paths.TryGetValue(normalizedPath, out var pathItem))
                {
                    pathItem = new OpenApiPathItem
                    {
                        Operations = new Dictionary<HttpMethod, OpenApiOperation>()
                    };
                    doc.Paths.Add(normalizedPath, pathItem);
                }

                pathItem.Operations[_opMap[routeMethod]] = new OpenApiOperation
                {
                    Summary = summary ?? "",
                    Parameters = pathParameters,
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse{ Description = "OK" }
                    },
                    Tags = new HashSet<OpenApiTagReference>() { new OpenApiTagReference(controller.Name) }
                };
            }
        }

        var sb = new StringBuilder();
        var writer = new OpenApiJsonWriter(new StringWriter(sb));
        doc.SerializeAsV3(writer);
        
        return sb.ToString();
    }

    private readonly Dictionary<string, HttpMethod> _opMap = new()
    {
        ["GET"] = HttpMethod.Get,
        ["get"] = HttpMethod.Get,

        ["POST"] = HttpMethod.Post,
        ["post"] = HttpMethod.Post,

        ["PUT"] = HttpMethod.Put,
        ["put"] = HttpMethod.Put,

        ["DELETE"] = HttpMethod.Delete,
        ["delete"] = HttpMethod.Delete,

        ["PATCH"] = HttpMethod.Patch,
        ["patch"] = HttpMethod.Patch,

        ["HEAD"] = HttpMethod.Head,
        ["head"] = HttpMethod.Head,

        ["OPTIONS"] = HttpMethod.Options,
        ["options"] = HttpMethod.Options,

        ["TRACE"] = HttpMethod.Trace,
        ["trace"] = HttpMethod.Trace
    };
}