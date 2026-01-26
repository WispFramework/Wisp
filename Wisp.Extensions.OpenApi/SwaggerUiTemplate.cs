// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Extensions.OpenApi;

public class SwaggerUiTemplate
{
    public static readonly string Template = """
                                        <!DOCTYPE html>
                                        <html lang="en">
                                          <head>
                                            <meta charset="utf-8" />
                                            <meta name="viewport" content="width=device-width, initial-scale=1" />
                                            <meta name="description" content="SwaggerUI" />
                                            <title>SwaggerUI</title>
                                            <link rel="stylesheet" href="https://unpkg.com/swagger-ui-dist@5.29.0/swagger-ui.css" />
                                          </head>
                                          <body>
                                          <div id="swagger-ui"></div>
                                          <script src="https://unpkg.com/swagger-ui-dist@5.29.0/swagger-ui-bundle.js" crossorigin></script>
                                          <script src="https://unpkg.com/swagger-ui-dist@5.29.0/swagger-ui-standalone-preset.js" crossorigin></script>
                                          <script>
                                            window.onload = () => {
                                            fetch('http://localhost:6969{#specPath#}')
                                              .then(res => res.json())
                                              .then(spec => {
                                                window.ui = SwaggerUIBundle({
                                                  //url: 'http://localhost:6969{#specPath#}',
                                                  spec: spec,
                                                  dom_id: '#swagger-ui',
                                                  presets: [
                                                    SwaggerUIBundle.presets.apis,
                                                    SwaggerUIStandalonePreset
                                                  ],
                                                  layout: "StandaloneLayout",
                                                });
                                              });
                                            };
                                          </script>
                                          </body>
                                        </html>
                                        """;
}