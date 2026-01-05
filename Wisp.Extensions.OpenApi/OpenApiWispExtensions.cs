// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wisp.Extensions.OpenApi.Config;
using Wisp.Framework;
using Wisp.Framework.Extensions;

namespace Wisp.Extensions.OpenApi;

public static class OpenApiWispExtensions
{
    public static WispHostBuilder UseOpenApi(this WispHostBuilder builder, Action<OpenApiConfigBuilder>? configBuilder = null)
    {
        var cb = new OpenApiConfigBuilder();
        configBuilder?.Invoke(cb);
        var config = cb.Build();
        config.Assembly ??= Assembly.GetExecutingAssembly();

        builder.Services.AddSingleton(config);
        builder.Services.AddSingleton<OpenApiGenerator>();
        
        return builder;
    }

    public static WispApplicationBuilder MapOpenApi(this WispApplicationBuilder appBuilder)
    {
        var config = appBuilder.Services.GetRequiredService<OpenApiConfig>();
        var generator = appBuilder.Services.GetRequiredService<OpenApiGenerator>();
        var logger = appBuilder.Services.GetRequiredService <ILogger<OpenApiGenerator>>();
        
        logger.LogInformation("OpenAPI Spec JSON available at {Path}", config.OpenApiSpecPath);
        logger.LogInformation("Swagger UI is available at {Path}", config.SwaggerUiPath);
        
        appBuilder.ConfigureRoutes(r =>
        {
            r.Get(config.OpenApiSpecPath, async (context) =>
            {
                var json = generator.GetSpecJson();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 200;
                
                var ms = new MemoryStream(json.AsUtf8Bytes());
                ms.Position = 0;
                context.Response.Body = ms;
            });

            r.Get(config.SwaggerUiPath, async (context) =>
            {
                var template = SwaggerUiTemplate.Template;
                template = template.Replace("{#specPath#}", config.OpenApiSpecPath);
                
                context.Response.ContentType = "text/html";
                context.Response.StatusCode = 200;
                
                var ms = new MemoryStream(template.AsUtf8Bytes());
                ms.Position = 0;
                context.Response.Body = ms;
            });
        });

        return appBuilder;
    }
}