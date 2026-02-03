namespace Wisp.Extensions.Admin;

public class Util
{
    private static readonly Dictionary<string, string> _irregulars = new()
    {
        { "index", "indices" },
        { "cactus", "cacti" },
        { "person", "people" },
        { "child", "children" },
        { "mouse", "mice" },
        { "goose", "geese" }
    };
    
    public static string Pluralize(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        // 1. Check for irregulars
        if (_irregulars.TryGetValue(name.ToLower(), out var irregular))
            return irregular;

        // 2. Words ending with 'y' preceded by consonant -> 'ies'
        if (name.Length > 1 && name.EndsWith("y", StringComparison.OrdinalIgnoreCase) &&
            !"aeiou".Contains(char.ToLower(name[^2])))
        {
            return name[..^1] + "ies";
        }

        // 3. Words ending with 's', 'x', 'z', 'ch', 'sh' -> add 'es'
        string lower = name.ToLower();
        if (lower.EndsWith("s") || lower.EndsWith("x") || lower.EndsWith("z") ||
            lower.EndsWith("ch") || lower.EndsWith("sh"))
        {
            return name + "es";
        }

        // 4. Default -> add 's'
        return name + "s";
    }
}