// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

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