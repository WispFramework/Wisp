---
icon: lucide/file-code-corner
---

# Templating

Wisp uses the [Fluid](https://github.com/sebastienros/fluid) templating engine,
which is a C# implementation of the [Shopify Liquid](https://shopify.github.io/liquid/)
templating language. Fluid is largely compatible with Liquid but we recommend
referring to the [unofficial Fluid documentation](https://deanebarker.net/tech/fluid/)
rather than the Liquid documentation.

## Loading Templates

By default, Wisp looks for template files in the `Templates/` directory relative to 
the current working directory (CWD).

Template files must have the `.liquid` extension. This is for compatibility reasons as
most modern IDEs support the Liquid language and the `.liquid` extension but most are not
aware of the Fluid language and if templates used `.html` or `.fluid`, you would potentially
lose syntax highlighting and other IDE features.

!!! warning
    When running your application with `dotnet run`, the current working directory (CWD) 
    is the project directory.

    When launching from an IDE such as Rider or Visual Studio, the CWD is usually set to
    the output directory (`bin/Debug/net10.0`) and so you need to set your template files
    to `CopyAlways` to copy them to the output directory and keep them up to date.

    You can do this automatically for all files in the Templates directory by putting
    this snippet in your `.csproj` file.

    ```xml
    <ItemGroup>
        <Content Include="Templates/**/*.*">
            <CopyToOutputDirectory>Always</CopyToOutputDirectory>
        </Content>
    </ItemGroup>
    ```

    For development, using `dotnet run` simplifies template hot-reloading.

### Template Hot-Reload

When running in `Debug` mode (e.g. with `dotnet run`), Wisp loads the templates from disk
for each request, so when you change a template file while the application is running, the
next request will use the updated version. No further action or configuration is required.

In `Release` mode, templates are cached in memory for performance and not reloaded automatically.

## Passing Data to Templates

When you return a `ViewResult` from a controller, you can optionally pass data to the template.
The data can be any object. A convenient concept to use here is 
[anonymous types](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/anonymous-types).

All public fields, properties and methods in the passed-in object are available in a template. Keep
in mind that the object is not passed to the template directly, instead, it's wrapped in a
`ViewModel` that also provides additional data to the template.

The object you pass to `View()` is available in the template under the `model` variable.

Here is a list of properties available in `ViewModel` (and therefore in a template).

| Property           | Name in Template     | Type                          | Description                                            |
|--------------------|----------------------|-------------------------------|--------------------------------------------------------|
| `UserLoggedIn`     | `user_logged_in`     | `bool`                        | Is the current user logged in?                         |
| `CurrentUserName`  | `current_user_name`  | `string?`                     | Current user's username                                |
| `CurrentUserRoles` | `current_user_roles` | `List<string>`                | Current user's roles                                   |
| `CurrentUserId`    | `current_user_id`    | `string`                      | Current user's ID                                      |
| `FlashMessages`    | `flash_messages`     | `List<FlashMessage>`          | Current [Flash Messages](../Middleware/flash-messages) |
| `Model`            | `model`              | `object?`                     | The passed-in model                                    |
| `Middleware`       | `middleware`         | `Dictionary<string, object?>` | Additional [Middleware](../Middleware) Data \*           |

*\* These values are populated per request and may be empty depending on which middleware components are active.*


Public member names are automatically translated to `snake_case` when exposed to the template.

## Layout

Every template can optionally use a layout. A layout is a special template that includes an
area where the inner template is rendered into.

To create a layout, create a new file, for example `_layout.liquid` and put `{% renderbody %}`
somewhere in the file. The `{% renderbody %}` tag is replaced with the content of the child template.

```html title="_layout.liquid"
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Hello Wisp</title>
</head>
<body>
    {% renderbody %}
</body>
```

Then use the layout with the `{% layout %}` directive. Keep in mind all paths in templates are
relative to the template root (`Templates/`) and there is no need to explicitly include the `.liquid` extension.

```html title="index.liquid"
{% layout '_layout' %}

<h1>Hello Wisp!</h1>
```

## Partials

You can include partial templates using the `{% include %}` directive. Paths are relative to the 
template root, and omit the `.liquid` extension.

Included partials have access to the same data as the template that included them.

```html title="index.liquid"
{% include '_header' }

<h1>Hello Wisp</h1>
```

## Macros

The Fluid templating engine supports macros. Macros are basically a way of creating your
own template directives.

!!! warning
    Macros are a feature of Fluid included for convenience but often considered a code smell.
    We recommend not using macros unless absolutely necessary (for example when proper 
    recursion is needed).

    **The general rule of thumb is that if you can't do it in a template without macros, it
    should probably be done in C#.**

See [this page](https://deanebarker.net/tech/fluid/functions/) for more information about macros.

You can define macros like so:

```html title="_macros.liquid"
{% macro hello_world(name) %}
<p>Hello {{name}}</p>
{% endmacro %}
```

And then include it and call it like so:

```html title="index.liquid"
{% from '_macros' import hello_world %}

{{ hello_world('Wisp') }}
```

## Custom Filters

!!! warning
    Templating configuration is not implemented yet.
    This section shows what this configuration may look like in the future.

In Liquid/Fluid, a filter is a function that transforms data in a template. 

For example: `#!handlebars {{ 'Hello World' | upcase }}` would render as `HELLO WORLD`.

The Fluid templating engine used in Wisp supports adding custom filters.

To add a custom filter, first create a static function somewhere with the following signature. The function can, but does
not have to be `async`.

```csharp
public static ValueTask<FluidValue> MyFilter(FluidValue input, FilterArguments args, TemplateContext context) {}
```

And then register it with the `WispHostBuilder.ConfigureTemplates` extension method.

```csharp
hostBuilder.ConfigureTemplates(t => 
{
    t.AddFilter("filterName", MyFilterClass.MyFilterMethod);
});
```


*[CWD]: Current Working Directory