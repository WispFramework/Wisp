using System.Reflection;

namespace Wisp.Extensions.OpenApi.Config;

public class OpenApiConfig
{
    public string Title { get; set; } = "";

    public string Version { get; set; } = "";
    
    public Assembly? Assembly { get; set; }
    
    public string OpenApiSpecPath { get; set; } = "/openapi/schema.json";
    
    public string SwaggerUiPath { get; set; } = "/openapi/swagger";
}