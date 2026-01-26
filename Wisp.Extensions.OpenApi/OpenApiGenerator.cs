// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
                if (routeAttr is null) continue;
                
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

                var methodParams = method.GetParameters();

                OpenApiRequestBody? requestBody = null;
                
                foreach (var param in methodParams)
                {
                    var fromBodyAttr = param.GetCustomAttribute<FromBodyAttribute>();
                    if (fromBodyAttr is not null)
                    {
                        var bodyType = param.ParameterType;
                        string? example = null;
                        
                        try
                        {
                            var instance = Activator.CreateInstance(bodyType);
                            if (instance is not null) example = JsonSerializer.Serialize(instance);
                        }
                        catch (Exception ex)
                        {
                            log.LogError(ex, "could not create an example for type");
                        }

                        var schema = GenerateSchema(bodyType);
                        requestBody = new OpenApiRequestBody
                        {
                            Required = true,
                            Content = new Dictionary<string, IOpenApiMediaType>
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = schema,
                                    Example = example
                                }
                            }
                        };
                        
                        continue;
                    }

                    if (!pathParameters.Any(p => p.Name == param.Name))
                    {
                        pathParameters.Add(new OpenApiParameter
                        {
                            Name = param.Name,
                            In = ParameterLocation.Query,
                            Required = true,
                            Schema = new OpenApiSchema { Type = JsonSchemaType.String }
                        });
                    }
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

                var innerReturnType = UnwrapInnermostGeneric(method.ReturnType);

                pathItem.Operations[_opMap[routeMethod]] = new OpenApiOperation
                {
                    Summary = summary ?? "",
                    Parameters = pathParameters,
                    Responses = new OpenApiResponses
                    {
                        ["200"] = new OpenApiResponse
                        {
                            Description = "OK",
                            Content = new Dictionary<string, IOpenApiMediaType>
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = GenerateSchema(innerReturnType)
                                }
                            }
                        }
                    },
                    Tags = new HashSet<OpenApiTagReference>() { new OpenApiTagReference(controller.Name) },
                    RequestBody = requestBody
                };
            }
        }

        var sb = new StringBuilder();
        var writer = new OpenApiJsonWriter(new StringWriter(sb));
        doc.SerializeAsV3(writer);
        
        return sb.ToString();
    }

    private static readonly HashSet<Type> KnownWrappers = new()
    {
        typeof(Task<>),
        typeof(ValueTask<>),
        typeof(ResultBox<>)
    };

    private static Type UnwrapInnermostGeneric(Type type)
    {
        while (true)
        {
            if (!type.IsGenericType) return type;
            var genericDef = type.GetGenericTypeDefinition();

            if (!KnownWrappers.Contains(genericDef))
                return type;

            type = type.GetGenericArguments()[0];
        }
    }

    private static OpenApiSchema GenerateSchema(Type type)
    {
        if (type == typeof(string))
            return new OpenApiSchema { Type = JsonSchemaType.String };
        
        if (type == typeof(int) || type == typeof(long))
            return new OpenApiSchema { Type = JsonSchemaType.Integer };

        if (type == typeof(bool))
            return new OpenApiSchema { Type = JsonSchemaType.Boolean };

        if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
            return new OpenApiSchema { Type = JsonSchemaType.Number };

        if (type == typeof(Guid)) return new OpenApiSchema { Type = JsonSchemaType.String };

        if (type.IsEnum) return new OpenApiSchema { Type = JsonSchemaType.String };
        
        if (type.IsArray)
        {
            return new OpenApiSchema
            {
                Type = JsonSchemaType.Array,
                Items = GenerateSchema(type.GetElementType()!)
            };
        }

        var props = new Dictionary<string, IOpenApiSchema>();
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            props[prop.Name] = GenerateSchema(prop.PropertyType);
        }

        return new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Properties = props
        };
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