// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.Text;

namespace Wisp.Framework.Extensions;

public static class StringExtensions
{
    public static byte[] AsUtf8Bytes(this string it) => Encoding.UTF8.GetBytes(it);
    
    public static string FromUtf8Bytes(this byte[] it) => Encoding.UTF8.GetString(it);
    
    public static string AsBase64Url(this byte[] it) => Convert.ToBase64String(it).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}