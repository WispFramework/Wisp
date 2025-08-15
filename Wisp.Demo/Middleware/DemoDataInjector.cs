using Wisp.Framework.Http;
using Wisp.Framework.Middleware;

namespace Wisp.Demo.Middleware;

public class DemoDataInjector(IMiddlewareDataInjector dataInjector) : HttpMiddleware
{
    public override Task OnRequestReceived(IHttpContext context)
    {
        dataInjector.Inject("demo", new { Hello = "World" });

        return Task.CompletedTask;
    }
}