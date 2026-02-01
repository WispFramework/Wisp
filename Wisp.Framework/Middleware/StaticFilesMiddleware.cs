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
using File = System.IO.File;

namespace Wisp.Framework.Middleware;

public class StaticFilesMiddleware(ILogger<StaticFilesMiddleware> log, IOptions<WispConfiguration> config, IHttpContextAccessor contextAccessor) : HttpMiddleware
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

    public override async Task OnRequestReceived()
    {
        var context = contextAccessor.HttpContext!;
        
        var configRoot = config.Value.StaticFileRoot;
        var allowIndexFiles = config.Value.AllowIndexFiles;

        var path = context.Request.Path.TrimStart('/');
        if (string.IsNullOrEmpty(path)) {
            if(!allowIndexFiles)
                return;
            
            path = "index.html";
        }
        path = path.Split('?', 2)[0];

        // if(path.EndsWith('/') && allowIndexFiles) path = path + "index.html";
        
        
        log.LogDebug("Looking for static file {File}", path);
        
        var root = Path.GetFullPath(configRoot);
        var reqPath = Path.GetFullPath(Path.Combine(root, path));

        if(Directory.Exists(reqPath) && allowIndexFiles) reqPath = Path.GetFullPath(Path.Combine(root, path, "index.html"));

        if (!reqPath.StartsWith(root))
        {
            context.Response.StatusCode = 403;
            context.IsHandled = true;
            return;
        }

        if (!File.Exists(reqPath)) return;
        
        log.LogDebug("static file found");

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