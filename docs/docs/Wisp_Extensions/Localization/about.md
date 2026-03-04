---
icon: lucide/languages
---

# About

The `Wisp.Extensions.I18n` extension adds localization functionality to Wisp.

!!! warning
    This extension is in early development stages. Some of the functionality described
    here may not be implemented yet. Expect many bugs.

## Installation

1. Install the `Wisp.Extensions.I18n` NuGet package
   ```shell
   dotnet add package Wisp.Extensions.I18n
   ```
2. Enable the I18n package in your `HostBuilder`
   ```csharp
   hostBuilder.AddI18n();
   ```
3. Create a `I18n/` directory in the root of your project
4. [Generate POT Files](#generating-translation-templates-pot)
5. Put your translated `.po` files in `I18n/`
6. You are ready to start translating your application!

## Generating Translation Templates (POT)

To write translations, you will need a translation template `(.pot)` file. You can generate a POT
file by calling `PotGenerator.Generate(string locale)`. The generator will parse your template files,
look for all usages of `gettext` and generate a template based on that.

To include the generation functionality in your app as a command, you can use the following snippet.

```csharp
if (args is ["gen-pot", _, ..])
{
    var locale = args[1];
    if (string.IsNullOrWhiteSpace(locale))
    {
        Console.WriteLine("Usage: ./tracker gen-pot <locale>");
        return;
    }
    
    var potBuilder = new PotGenerator();
    Console.WriteLine("Generating...");
    potBuilder.Generate(locale);
    Console.WriteLine("Done");
    
    return;
}
```

You can then generate template files by running `dotnet run -- gen-pot <locale>`.

## Editing `.po` Files

There are many `.po` file editors available. We recommend [Poedit](https://poedit.net/).

## Switching Languages

The extension includes middleware that detects the desired locale from a query parameter or headers.

By default, wisp will look for the desired locale in the `?lang` query parameter and the
`X-Wisp-Locale` header, in that order.

For information about implementing custom locale detection, see [Injecting Locale](injecting-locale.md).