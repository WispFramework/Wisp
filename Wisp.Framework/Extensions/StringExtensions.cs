using System.Text;

namespace Wisp.Framework.Extensions;

public static class StringExtensions
{
    public static byte[] AsUtf8Bytes(this string it) => Encoding.UTF8.GetBytes(it);
}