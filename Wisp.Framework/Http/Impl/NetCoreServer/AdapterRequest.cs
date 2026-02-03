// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.Net;
using System.Text.Json.Serialization;
using System.Web;
using NetCoreServer;

namespace Wisp.Framework.Http.Impl.NetCoreServer;

/// <summary>
/// This is the NetCoreServer implementation of IHttpRequest
/// </summary>
public class AdapterRequest : IHttpRequest
{
    /// <summary>
    /// This is the NetCoreServer implementation of IHttpRequest
    /// </summary>
    /// <param name="req"></param>
    public AdapterRequest(HttpRequest req)
    {
        Path = req.Url;
        Method = req.Method;
        Headers = req.GetHeaders().ToDictionary();

        if (Path.Contains('?'))
        {
            var parsed = HttpUtility.ParseQueryString(Path.Split('?', 2)[1]);
            var dic = parsed.AllKeys.ToDictionary(k => k ?? "unnamed", k => parsed[k] ?? "");
            QueryParams = dic;
        }

        QueryParams ??= new Dictionary<string, string>();

        Body = new MemoryStream(req.BodyBytes ?? []);
        Cookies = req.GetCookies().ToDictionary();
    }


    public string Id { get; } = Guid.NewGuid().ToString();
    
    public string Method { get; }

    public string Path { get; }

    public IPEndPoint ClientEndpoint { get; set; }

    public IReadOnlyDictionary<string, string> Headers { get; }

    public IReadOnlyDictionary<string, string> QueryParams { get; }

    public Dictionary<string, string> PathVars { get; set; }
    
    public Dictionary<string, string> FormData { get; set; }

    public List<File> Files { get; set; } = new();

    public string ContentType { get; set; } = "application/octet-stream";

    [JsonIgnore]
    public Stream Body { get; set; }

    public IReadOnlyDictionary<string, string> Cookies { get; }

    public string ReadBodyAsString()
    {
        using var reader = new StreamReader(Body);
        return reader.ReadToEnd();
    }

    public async Task<string> ReadBodyAsStringAsync()
    {
        using var reader = new StreamReader(Body);
        return await reader.ReadToEndAsync();
    }
}