using System.Text;
using System.Text.RegularExpressions;
using GlobExpressions;
using Karambolo.PO;

namespace Wisp.Extensions.L10n;

public class PotGenerator()
{
    private static readonly Regex GettextFilter =
        new(@"\{\{\s*([""'])(?<msg>(?:\\.|(?!\1).)*?)\1\s*\|\s*gettext\b.*?\}\}",
            RegexOptions.Compiled);
    
    public void Generate(string locale)
    {
        var templateDir = Path.Combine(Environment.CurrentDirectory, "Templates");
        if (!Directory.Exists(templateDir))
        {
            Console.WriteLine("Can't build POT files, no Templates directory found");
            return;
        }
        
        var templateFiles = Glob.Files(templateDir, "**/*.liquid").ToList();
        if (templateFiles.Count == 0)
        {
            Console.WriteLine("Can't build POT files, not templates found");
            return;
        }

        var catalog = new POCatalog
        {
            Encoding = "UTF-8",
            Headers = new Dictionary<string, string>
            {
                ["Content-Type"] = "text/plain; charset=UTF-8"
            }
        };

        var index = new Dictionary<POKey, POSingularEntry>();

        foreach (var file in templateFiles)
        {
            var realPath = Path.Combine(templateDir, file);
            
            if (!File.Exists(realPath))
            {
                Console.WriteLine($"File {realPath} not found");
                continue;
            }

            var text = File.ReadAllText(realPath);

            foreach (Match m in GettextFilter.Matches(text))
            {
                var raw = m.Groups["msg"].Value;
                var msgid = Unescape(raw);
                
                Console.WriteLine($"Found {raw} || {msgid}");

                // Skip empty
                if (string.IsNullOrWhiteSpace(msgid))
                    continue;

                var line = 1 + text.AsSpan(0, m.Index).Count('\n');

                var key = new POKey(msgid); // contextless. Use new POKey(context, msgid) if you add msgctxt.

                if (!index.TryGetValue(key, out var entry))
                {
                    entry = new POSingularEntry(key)
                    {
                        // POT: translations intentionally empty
                        Translation = string.Empty
                    };

                    catalog.Add(entry);
                    index[key] = entry;
                }
            }
        }

        using var writer = new StreamWriter(Path.Combine(Environment.CurrentDirectory, "L10n", $"{locale}.pot"), false, Encoding.UTF8);

        var generator = new POGenerator(new POGeneratorSettings());
        generator.Generate(writer, catalog);
    }
    
    private static string Unescape(string s) =>
        s.Replace("\\n", "\n")
            .Replace("\\t", "\t")
            .Replace("\\r", "\r")
            .Replace("\\\"", "\"")
            .Replace("\\'", "'")
            .Replace("\\\\", "\\");
}