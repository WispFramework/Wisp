// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.Text.Json;

namespace Wisp.Framework.Controllers;

public static class ControllerResultSerializer
{
    public static (string Content, bool IsSimple) Serialize<T>(T value)
    {
        var type = typeof(T);

        if (IsSimpleType(type))
        {
            return (value?.ToString() ?? "null", true);
        }
        else
        {
            return (JsonSerializer.Serialize(value), false);
        }
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive
               || type.IsEnum
               || type == typeof(string)
               || type == typeof(decimal)
               || type == typeof(DateTime)
               || type == typeof(DateTimeOffset)
               || type == typeof(Guid)
               || Nullable.GetUnderlyingType(type) is Type underlying && IsSimpleType(underlying);
    }
}