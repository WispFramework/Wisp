using System.Text;

namespace Wisp.Framework.Extensions;

public static class StringExtensions
{
    public static byte[] AsUtf8Bytes(this string it) => Encoding.UTF8.GetBytes(it);
    
    public static string FromUtf8Bytes(this byte[] it) => Encoding.UTF8.GetString(it);
    
    public static string AsBase64Url(this byte[] it) => Convert.ToBase64String(it).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}