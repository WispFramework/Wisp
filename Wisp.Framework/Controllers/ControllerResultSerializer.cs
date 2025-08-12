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