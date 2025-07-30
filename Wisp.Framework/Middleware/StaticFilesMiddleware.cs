// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wisp.Framework.Configuration;
using Wisp.Framework.Http;

namespace Wisp.Framework.Middleware;

public class StaticFilesMiddleware(ILogger<StaticFilesMiddleware> log, IOptions<WispConfiguration> config) : HttpMiddleware
{
    private static readonly Dictionary<string, string> MimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        {".txt", "text/plain"},
        {".html", "text/html"},
        {".htm", "text/html"},
        {".css", "text/css"},
        {".js", "application/javascript"},
        {".json", "application/json"},
        {".png", "image/png"},
        {".jpg", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".gif", "image/gif"},
        {".svg", "image/svg+xml"},
        {".ico", "image/x-icon"},
        {".woff", "font/woff"},
        {".woff2", "font/woff2"},
        {".ttf", "font/ttf"},
        {".eot", "application/vnd.ms-fontobject"},
        {".mp4", "video/mp4"},
        {".webm", "video/webm"},
        {".ogg", "audio/ogg"}
    };

    public override async Task OnRequestReceived(IHttpContext context)
    {
        var configRoot = config.Value.StaticFileRoot;

        var path = context.Request.Path.TrimStart('/');
        if (string.IsNullOrEmpty(path)) return;
        path = path.Split('?', 2)[0];
        
        var root = Path.GetFullPath(configRoot);
        var reqPath = Path.GetFullPath(Path.Combine(root, path));

        if (!reqPath.StartsWith(root))
        {
            context.Response.StatusCode = 403;
            context.IsHandled = true;
            return;
        }

        if (!File.Exists(reqPath)) return;

        var ext = Path.GetExtension(reqPath);
        var contentType = MimeTypes.TryGetValue(ext, out var mime) ? mime : "application/octet-stream";

        var content = await File.ReadAllBytesAsync(reqPath);

        context.Response.StatusCode = 200;
        context.Response.ContentType = contentType;

        context.Response.Body = new MemoryStream(content);
        context.IsHandled = true;

        log.LogDebug("found static file '{Root}/{FileName}'", configRoot, path);
    }
}