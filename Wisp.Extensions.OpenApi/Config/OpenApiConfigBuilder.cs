using System.Reflection;

namespace Wisp.Extensions.OpenApi.Config;

public class OpenApiConfigBuilder
{
    private OpenApiConfig config = new OpenApiConfig();

    public OpenApiConfigBuilder Title(string title)
    {
        config.Title = title;
        return this;
    }

    public OpenApiConfigBuilder Version(string version)
    {
        config.Version = version;
        return this;
    }

    public OpenApiConfigBuilder InAssembly(Assembly assembly)
    {
        config.Assembly = assembly;
        return this;
    }

    public OpenApiConfigBuilder OpenApiSpecPath(string path)
    {
        config.OpenApiSpecPath = path;
        return this;
    }

    public OpenApiConfigBuilder SwaggerUiPath(string path)
    {
        config.SwaggerUiPath = path;
        return this;
    }
    
    public OpenApiConfig Build() => config;
}