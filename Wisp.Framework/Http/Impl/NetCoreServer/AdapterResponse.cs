// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.Text.Json.Serialization;
using NetCoreServer;

namespace Wisp.Framework.Http.Impl.NetCoreServer;

/// <summary>
/// This is the NetCoreServer implementation of IHttpResponse
/// </summary>
/// <param name="session"></param>
public class AdapterResponse(HttpSession session) : IHttpResponse
{

    private readonly HttpSession _session = session;

    public int StatusCode { get; set; } = 200;

    public string ContentType { get; set; } = "text/plain";

    public Dictionary<string, string> Headers { get; set; } = new();

    public Dictionary<string, string> Cookies { get; set; } = new();

    [JsonIgnore]
    public Stream Body { get; set; } = new MemoryStream();
}