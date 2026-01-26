---
icon: lucide/zap
---

# Flash Messages

Flash messages are basically read-once messages attached to a controller
response. They are attached to a user session and kept in memory until read.

Flash messages can persist through multiple redirects as long as nothing
reads them in the process.

## Enabling Flash Messages

To enable flash messages, you will need to register the Flash Message
[Middleware](../Middleware). The easiest way to do this is with the
`UseFlashMessages()` extension method.

The Flash Message Middleware requires a [Session Store](../Middleware/sessions),
and will register an `InMemorySessionStore` if no other session store is found.

```csharp
hostBuilder.UseFlashMessages();
```

## Displaying Flash Messages

Any flash messages for the current session are available from the
`flash_messages` variable in templates. Each message is an instance
of `FlashMessage` and has a `message` and `type` fields.

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
[Route("/")]
public class IndexController(FlashService flashService) : ControllerBase 
{
    [HttpGet]
    [Route("")]
    public ViewResult GetIndex() 
    {
        flashService.AddFlashMessage("Hello World!", FlashMessageType.Info);
        return View("index");
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

The `FlashService#AddFlashMessage` method has an overload that accepts a string
for the type. You can pass in any custom value.

```csharp
flashService.AddFlashMessage("Hey there!", "greeting");
```