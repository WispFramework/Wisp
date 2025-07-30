// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.Text;
using System.Text.Json;

namespace Wisp.Framework.Http;

/// <summary>
/// This is the only IHttpResponse implementation that should be directly instantiated
/// </summary>
public class WispHttpResponse : IHttpResponse
{
    public int StatusCode { get; set; } = 200;

    public string ContentType { get; set; } = "text/plain";

    public Dictionary<string, string> Headers { get; set; } = new();

    public Dictionary<string, string> Cookies { get; set; } = new();

    public Stream Body { get; set; }

    /// <summary>
    /// A convenience function that quickly instantiates a 200 OK response
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static WispHttpResponse Ok(object content)
    {
        var res = new WispHttpResponse();
        if (content is string s)
        {
            res.Body = new MemoryStream(Encoding.UTF8.GetBytes(s));
        }
        else
        {
            var json = JsonSerializer.Serialize(content);
            res.Body = new MemoryStream(Encoding.UTF8.GetBytes(json));
            res.ContentType = "application/json";
        }

        return res;
    }
}