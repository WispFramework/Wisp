// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Extensions;

public static class CollectionExtensions
{
    public static T? GetOrDefaultIgnoreCase<T>(this IDictionary<string, T> it, string key)
        => it.FirstOrDefault(kvp => kvp.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Value ?? default;
    
    public static T? GetOrDefaultIgnoreCaseReadonly<T>(this IReadOnlyDictionary<string, T> it, string key)
        => it.FirstOrDefault(kvp => kvp.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Value ?? default;
}