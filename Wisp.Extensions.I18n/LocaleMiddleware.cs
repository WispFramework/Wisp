using Microsoft.Extensions.Logging;
using Wisp.Framework.Http;
using Wisp.Framework.Views;

namespace Wisp.Extensions.I18n;

public class LocaleMiddleware(IHttpContextAccessor contextAccessor, ILogger<LocaleMiddleware> log) : HttpMiddleware
{

    public override Task OnTemplateRendering(ViewModel model)
    {
        var ctx = contextAccessor.HttpContext;
        if (ctx is null)
        {
            log.LogWarning("No context");
            return Task.CompletedTask;
        }

        if (ctx.Request.QueryParams.TryGetValue("lang", out var lang))
        {
            log.LogTrace("Found locale in lang query: {Lang}", lang);
            model.Locale = lang;
            return Task.CompletedTask;
        }

        if (ctx.Request.Headers.TryGetValue("X-Wisp-Lang", out var headerLang))
        {
            log.LogTrace("Found locale in header: {Lang}", headerLang);
            model.Locale = headerLang;
            
        }

        return Task.CompletedTask;
    }
}