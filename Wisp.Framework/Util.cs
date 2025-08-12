namespace Wisp.Framework;

public static class Utils
{
    public static bool IsUnboundGenericInstance(object? obj, Type openGenericType)
    {
        if (obj == null)
            return false;

        if (openGenericType == null)
            throw new ArgumentNullException(nameof(openGenericType));

        if (!openGenericType.IsGenericTypeDefinition)
            throw new ArgumentException("Must be an open generic type definition", nameof(openGenericType));

        var type = obj.GetType();

        while (type != null && type != typeof(object))
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == openGenericType)
                return true;

            type = type.BaseType!;
        }

        foreach (var iface in obj.GetType().GetInterfaces())
        {
            if (iface.IsGenericType && iface.GetGenericTypeDefinition() == openGenericType)
                return true;
        }

        return false;
    }
}