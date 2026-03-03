---
icon: lucide/zap
---

# Flash Messages

Flash messages are read-once messages stored in the user session and attached to a response.

Flash messages persist through redirects until they are accessed. 
Once read (for example, during template rendering), they are removed.

## Enabling Flash Messages

To enable flash messages, you will need to register the Flash Message
[Middleware](../Middleware). The easiest way to do this is with the
`UseFlashMessages()` extension method.

The Flash Message Middleware requires a [Session Store](../Middleware/sessions).
If no session store is registered, Wisp automatically registers an `InMemorySessionStore`.

```csharp
hostBuilder.UseFlashMessages();
```

## Lifecycle

 1. A controller calls `AddFlashMessage`.
 2. The message is stored in the session.
 3. On the next request, the message is exposed to templates via `flash_messages`.
 4. After being read, it is removed.

## Displaying Flash Messages

Any flash messages for the current session are available from the
`flash_messages` variable in templates. Each message is an instance
of `FlashMessage`.

Each message has:

- `message` – the text content
- `type` – a lowercase string (e.g. `info`, `error`) typically used for CSS styling

You can use these to display flash messages in your templates.

```html title="index.liquid"
{% if flash_messages %}
    <div class="container">
        {% for message in flash_messages %}
            <div class="alert alert-{{ message.type }}">{{ message.message }}</div>
        {% endfor %}
    </div>
{% endif %}
```

## Sending Flash Messages

To create a new flash message and attach it to the session, you will need to
get an instance of `FlashService` from DI and then call the `AddFlashMessage`
method.

```csharp
[Controller]
public class IndexController(FlashService flashService) : ControllerBase 
{
    [HttpGet]
    [Route("/hello")]
    public ViewResult GetHello() 
    {
        flashService.AddFlashMessage("Hello World!", FlashMessageType.Info);
        return Redirect("/");
    }
}
```

## Message Types

There are 5 predefined message types in the `FlashService.FlashMessageType`
enum that translate into a string like this:

 - `Primary` => `primary`
 - `Info` => `info`
 - `Warning` => `warning`
 - `Success` => `success`
 - `Error` => `error`

The `FlashService.AddFlashMessage` method has an overload that accepts a string
for the type. You can pass in any custom value.

```csharp
flashService.AddFlashMessage("Hey there!", "greeting");
```