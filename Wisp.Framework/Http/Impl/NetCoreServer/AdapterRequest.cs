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
/// <param name="req"></param>
public class AdapterRequest(HttpRequest req) : IHttpRequest
{
    public string Id { get; } = Guid.NewGuid().ToString();
    
    public string Method => req.Method;

    public string Path { get; } =  req.Url;

    public IPEndPoint ClientEndpoint { get; set; }

    public IReadOnlyDictionary<string, string> Headers => req.GetHeaders();

    public IReadOnlyDictionary<string, string> QueryParams
    {
        get
        {
            if (Path.Contains('?'))
            {
                var parsed = HttpUtility.ParseQueryString(Path.Split('?', 2)[1]);
                var dic = parsed.AllKeys.ToDictionary(k => k ?? "unnamed", k => parsed[k] ?? "");
                return dic;
            }
            return new Dictionary<string, string>();
        }
    }

    public Dictionary<string, string> PathVars { get; set; }
    
    public Dictionary<string, string> FormData { get; set; }

    [JsonIgnore]
    public Stream Body => new MemoryStream(req.BodyBytes ?? []);

    public IReadOnlyDictionary<string, string> Cookies => req.GetCookies();

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