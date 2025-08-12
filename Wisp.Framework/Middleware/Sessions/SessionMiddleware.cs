using Microsoft.Extensions.Logging;
using Wisp.Framework.Http;

namespace Wisp.Framework.Middleware.Sessions;

public class SessionMiddleware(ISessionStore store, ILogger<SessionMiddleware> log) : HttpMiddleware
{
    public override async Task OnRequestReceived(IHttpContext context)
    {
        if (context.Request.Cookies.TryGetValue("WISP_SESSION", out var sessionCookie))
        {
            log.LogDebug("Session cookie found ({Val})", sessionCookie);
            var session = await store.GetAsync(sessionCookie);
            if (session != null)
            {
                log.LogDebug("session found");
                context.Session = session;
                return;
            }
        }

        log.LogDebug("session not found");
        var newSession = await store.CreateAsync();
        context.Session = newSession;
        context.Response.Cookies["WISP_SESSION"] = newSession.Id;
    }
}