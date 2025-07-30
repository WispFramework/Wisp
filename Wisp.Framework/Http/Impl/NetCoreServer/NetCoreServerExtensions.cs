// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using NetCoreServer;

namespace Wisp.Framework.Http.Impl.NetCoreServer;

/// <summary>
/// Convenience extensions for the NetCoreServer HTTP backend
/// </summary>
public static class NetCoreServerExtensions
{
    public static Dictionary<string, string> GetHeaders(this HttpRequest req)
    {
        var count = req.Headers;
        var output = new Dictionary<string, string>();

        for (int i = 0; i < count; i++)
        {
            var (k, v) = req.Header(i);
            output[k] = v;
        }

        return output;
    }

    public static Dictionary<string, string> GetCookies(this HttpRequest req)
    {
        var count = req.Cookies;
        var output = new Dictionary<string, string>();

        for (int i = 0; i < count; i++)
        {
            var (k, v) = req.Cookie(i);
            output[k] = v;
        }

        return output;
    }
}