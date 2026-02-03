using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Wisp.Extensions.Admin.AdminSiteGenerator;
using Wisp.Extensions.Admin.Attributes;
using Wisp.Extensions.Admin.Generators;
using Wisp.Framework;

namespace Wisp.Extensions.Admin;

public static class WispExtensions
{
    extension(WispHostBuilder builder)
    {
        public WispHostBuilder AddAdmin(Action<AdminConfigBuilder> configBuilder)
        {
            var cb = new AdminConfigBuilder();
            configBuilder.Invoke(cb);
            var config = cb.Build();
            
            builder.Services.AddSingleton(config);
            builder.Services.AddSingleton<SchemaBuilder>();
            builder.Services.AddSingleton<ActionMapper>();
            builder.Services.AddSingleton<ListViewRenderer>();

            return builder;
        }
    }

    extension(WispApplicationBuilder builder)
    {
        public WispApplicationBuilder MapAdminRoutes(Assembly? assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly();
            
            var mapper = builder.Services.GetRequiredService<ActionMapper>();
            mapper.MapAll(assembly.GetTypes().Where(t => t.GetCustomAttribute<GenerateCrudAttribute>() != null).ToList());

            #if DEBUG
            HotReloadHandler.PostUpdateApplicationEvent += _ =>
            {
                mapper.MapAll(assembly.GetTypes().Where(t => t.GetCustomAttribute<GenerateCrudAttribute>() != null).ToList());
            };
            #endif
            
            return builder;
        }
    }
}