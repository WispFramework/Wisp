// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.Text.Json.Serialization;

namespace Wisp.Framework.Http;

/// <summary>
/// A wrapper object for HTTP requests
/// </summary>
public interface IHttpRequest
{
    /// <summary>
    /// A unique ID for each request
    /// </summary>
    string Id { get; }
    
    /// <summary>
    /// The HTTP Method
    /// </summary>
    string Method { get; }

    /// <summary>
    /// The full request path, without the server name
    /// </summary>
    string Path { get; }

    /// <summary>
    /// A list of HTTP headers
    /// </summary>
    IReadOnlyDictionary<string, string> Headers { get; }

    /// <summary>
    /// A list of HTTP cookies
    /// </summary>
    IReadOnlyDictionary<string, string> Cookies { get; }
    
    /// <summary>
    /// A list of parsed query parameters
    /// </summary>
    IReadOnlyDictionary<string, string> QueryParams { get; }
    
    /// <summary>
    /// A list of parsed path variables
    /// </summary>
    Dictionary<string, string> PathVars { get; set; }
    
    Dictionary<string, string> FormData { get; set; }

    /// <summary>
    /// The request body
    /// </summary>
    [JsonIgnore]
    Stream Body { get; }

    /// <summary>
    /// Read the body stream into a string
    /// </summary>
    /// <returns></returns>
    string ReadBodyAsString();
    
    /// <summary>
    /// Read the body stream into a string asynchronously
    /// </summary>
    /// <returns></returns>
    Task<string> ReadBodyAsStringAsync();
}