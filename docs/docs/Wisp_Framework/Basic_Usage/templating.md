---
icon: lucide/file-code-corner
---

# Templating

Wisp uses the [Fluid](https://github.com/sebastienros/fluid) templating engine,
which is a C# implementation of the [Shopify Liquid](https://shopify.github.io/liquid/)
templating language. Fluid is mostly fully compatible with Liquid but we recommend
using [this unofficial Liquid documentation](https://deanebarker.net/tech/fluid/)
rather than the Fluid documentation.

## Loading Templates

By default, Wisp looks for template files in the `Templates/` directory in the current
CWD.

Template files must have the `.liquid` extension. This is for compatibility reasons as
most modern IDEs support the Liquid language and the `.liquid` extension but most are not
aware of the Fluid language and if templates used `.html` or `.fluid`, you would potentially
lose syntax highlighting and other IDE features.

!!! warning
    When running your application with `dotnet run`, the CWD for the process is the
    directory you launched it from.

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

    That being said, **we recommend using `dotnet run`**, it makes reloading templates
    much easier.

### Template Hot-Reload

When running in `Debug` mode (e.g. with `dotnet run`), Wisp loads the templates from disk
for each request, so when you change a template file while the application is running, the
next request will use the updated version. No further action or configuration is required.

In `Release` mode, Wisp caches templates aggressively so to apply changes, you will need to
restart the application.

## Passing Data to Templates

When you return a `ViewResult` from a controller, you can optionally pass data to the template.
The data can be any instance of `object`.

All public fields, properties and methods in the passed object are available in a template. Keep
in mind that the object is not passed to the template directly, instead, it's wrapped in a
`ViewModel` that also provides additional data to the template.

Here is a list of properties available in `ViewModel` (and therefore in a template).

| Property       | Name in Template | Type   | Description                    |
|----------------|------------------|--------|--------------------------------|
| `UserLoggedIn` | `user_logged_in` | `bool` | Is the current user logged in? |
| `CurrentUserName` | `current_user_name` | `string?` | Current user's username |
| `CurrentUserRoles` | `current_user_roles` | `List<string>` | Current user's roles |
| `CurrentUserId` | `current_user_id` | `string` | Current user's ID |
| `FlashMessages` | `flash_messages` | `List<FlashMessage>` | Current [Flash Messages](../Middleware/flash-messages) |
| `Model` | `model` | `object?` | The passed-in model |
| `Middleware` | `middleware` | `Dictionary<string, object?>` | Additional [Middleware](../Middleware) Data |


To align
with naming conventions of the Fluid language, member names are translated to `snake_case`.

## Layout

Every template can optionally use a layout. A layout is a special template that includes an
area where the inner template is rendered into.

To create a layout, create a new file, for example `_layout.liquid` and put `{% renderbody %}`
somewhere in the file.

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

Then, use the layout with the `{% layout %}` directive. Keep in mind all paths in templates are
relative and there is no need to explicitly include the `.liduid` extension.

```html title="index.liquid"
{% layout '_layout' %}

<h1>Hello Wisp!</h1>
```

## Partials

You can include partial templates using the `{% include %}` directive. As with layouts, the
path is relative and you don't need to specify the extensions.

Included partials have access to the same data as the template that included them.

```html title="index.liquid"
{% include '_header' }

<h1>Hello Wisp</h1>
```

## Macros

The Fluid templating engine supports macros. Macros are basically a way of creating your
own template directives.

!!! warning
    Macros are a feature of Fluid included for convenience but generally considered somewhat
    unsafe and a code smell. We recommend not using macros unless absolutely neccessary 
    (for example when proper recursion is needed).

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

*[CWD]: Current Working Directory