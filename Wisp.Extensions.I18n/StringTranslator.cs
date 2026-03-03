using GlobExpressions;
using Karambolo.PO;
using Microsoft.Extensions.Logging;

namespace Wisp.Extensions.I18n;

public class StringTranslator
{
    private readonly ILogger<StringTranslator> _log;

    private readonly Dictionary<string, POCatalog> _catalogs = [];

    private readonly Dictionary<string, List<string>> _unknowns = [];

    public StringTranslator(ILogger<StringTranslator> log)
    {
        _log = log;
        var locDir = Path.Combine(Environment.CurrentDirectory, "L10n");
        if (!Directory.Exists(locDir))
        {
            log.LogWarning("Cannot initialize StringTranslator, the L10n/ directory doesn't exist");
            return;
        }

        var poFiles = Glob.Files(locDir, "**/*.po").ToList();
        if (poFiles.Count == 0)
        {
            log.LogWarning("No .po translation files found in L10n/");
            return;
        }

        foreach (var poFile in poFiles)
        {
            var realPath = Path.Combine(locDir, poFile);
            log.LogInformation("Loading translations from {File}", realPath);
            LoadPoFile(realPath);
        }

        foreach (var (lang, _) in _catalogs)
        {
            log.LogInformation("Loaded language file for {Lang}", lang);
        }
    }

    private void LoadPoFile(string path)
    {   
        using var reader = new StreamReader(path);
        var parser = new POParser();
        var result = parser.Parse(reader);

        if (!result.Success)
        {
            foreach (var diag in result.Diagnostics)
            {
                _log.LogError("PO Loading Error: {Error}", diag);
            }
        }

        var catalog = result.Catalog;
        _catalogs[catalog.Language.ToLowerInvariant()] = catalog;
        
        _log.LogInformation("Loaded locale {Locale} from {File} with {Keys} keys", catalog.Language, path, catalog.Keys.Count());
    }

    public string? GetTranslation(string locale, string text)
    {
        if (_catalogs.TryGetValue(locale, out var catalog))
        {
            return catalog.GetTranslation(new POKey(text));
        }

        return null;
    }

    public string? GetTranslationWithContext(string locale, string context, string text)
    {
        if (_catalogs.TryGetValue(locale, out var catalog))
        {
            return catalog.GetTranslation(new POKey(text, contextId: context));
        }

        return null;
    }

    public string? GetTranslationWithPlural(string locale, string text, string plural, int n)
    {
        if (_catalogs.TryGetValue(locale, out var catalog))
        {
            return catalog.GetTranslation(new POKey(text, pluralId: plural), n);
        }

        return null;
    }
}