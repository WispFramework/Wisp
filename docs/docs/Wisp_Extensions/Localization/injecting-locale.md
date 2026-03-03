---
icon: lucide/syringe
---

# Injecting Locale

To override the default locale detection methods, you will need to implement a middleware
with the `OnTemplateRendering` method and inject your locale string into the `Locale`
property of the template `ViewModel`.

**This is currently the only supported method.**

```csharp
public class LocaleMiddleware : HttpMiddleware 
{
    public override Task OnTemplateRendering(ViewModel viewModel) 
    {
        var locale = MyLocaleDetectingMethod();
        viewModel.Locale = locale;
        
        return Task.CompletedTask();
    }
}
```