namespace Wisp.Framework.Http;

public static class ErrorPageRenderer
{
    private const string Template = @"
<!doctype html>
<html lang=""en"">
    <head>
        <meta charset=""utf-8"">
        <title>Application Error</title>
        <style>
            body {
                font-family: sans-serif;
                padding: 4em;
            }

            h1 {
                color: #dc2626;
                font-weight: normal;
            }

            code {
                background-color: #f3f4f6;
            }

            pre {
                padding: 1em;
                background-color: #f3f4f6;
                font-family: monospace;
                overflow-y: scroll;
            }
        </style>
    </head>
    <body>
        <h1>An unhandled exception occured while processing the request.</h1>
        <h2 style=""font-weight: normal;""><strong>Location:</strong> {{ path }}</h2>
        <h2>{{ exceptionType }}: {{ exceptionMessage }}</h2>
        <hr>
        <code><pre>{{ exceptionMessage }}
{{ exceptionStackTrace }}</pre></code>
    </body>
</html>
";
    
    public static string RenderErrorPage(Exception ex, string? path = "/")
        => Template
            .Replace("{{ exceptionMessage }}", ex.Message)
            .Replace("{{ exceptionLocation }}", $"{ex.Source}: {ex.TargetSite?.Name}")
            .Replace("{{ exceptionType }}", ex.GetType().Name)
            .Replace("{{ exceptionStackTrace }}", ex.StackTrace?
                .Replace("<", "&lt;").Replace(">", "&gt;"))
            .Replace("{{ path }}", path);
}