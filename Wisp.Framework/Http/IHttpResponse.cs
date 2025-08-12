// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.Text.Json.Serialization;

namespace Wisp.Framework.Http;

/// <summary>
/// A wrapper object for HTTP responses
/// </summary>
public interface IHttpResponse
{
    /// <summary>
    /// HTTP Status Code
    /// <remarks>See https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Status</remarks>
    /// </summary>
    int StatusCode { get; set; }

    /// <summary>
    /// The HTTP Content-Type
    /// <remarks>See https://developer.mozilla.org/en-US/docs/Web/HTTP/Reference/Headers/Content-Type</remarks>
    /// </summary>
    string ContentType { get; set; }

    /// <summary>
    /// A list of HTTP headers
    /// </summary>
    Dictionary<string, string> Headers { get; set; }

    /// <summary>
    /// A list of HTTP cookies
    /// </summary>
    Dictionary<string, string> Cookies { get; set; }

    /// <summary>
    /// The response body
    /// </summary>
    [JsonIgnore]
    Stream Body { get; set; }
}