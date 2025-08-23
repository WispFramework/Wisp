namespace Wisp.Framework.Extensions;

public static class CollectionExtensions
{
    public static T? GetOrDefaultIgnoreCase<T>(this IDictionary<string, T> it, string key)
        => it.FirstOrDefault(kvp => kvp.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Value ?? default;
}