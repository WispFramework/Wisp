---
icon: lucide/book-type
---

# GetText

The extension implements `gettext`-like functionality for translating templates and other
strings.

## `gettext` in templates

You can use the `gettext` filter to translate any string in your templates. There is no additional
configuration required.

### Basic Usage

```liquid
<h1>{{ 'Hello World' | gettext }}</h1>
```

### Usage with Context

*not yet implemented*

You may optionally specify context as the first argument.

```liquid
<h1>{{ 'Hello World' | gettext: 'homepage' }}</h1>
```

### Plural Usage

*not yet implemented*

For plural forms, use the `gettext_pl` filter, where the 1st argument is the plural form
and the 2nd argument is the amount.

```liquid
<h1>{{ 'Hello World' | gettext_pl: 'Hello {0} Worlds', 4 }}</h1>
```