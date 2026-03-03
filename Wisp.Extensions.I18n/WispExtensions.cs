using Microsoft.Extensions.DependencyInjection;
using Wisp.Framework;

namespace Wisp.Extensions.I18n;

public static class WispExtensions
{
    extension(WispHostBuilder builder)
    {
        public WispHostBuilder AddI18n()
        {
            builder.AddDoLast(b =>
            {
                b.Services.AddSingleton<StringTranslator>();
                b.Services.AddSingleton<TemplateFilters>();
                b.AddMiddleware<LocaleMiddleware>();
            
                var instance = b.Services.BuildServiceProvider().GetRequiredService<TemplateFilters>();
                b.AddTemplateFilter("gettext", instance.GetText);
            });

            return builder;
        }
    }
}