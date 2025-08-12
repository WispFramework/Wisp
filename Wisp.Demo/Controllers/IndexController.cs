using Wisp.Framework.Controllers;
using Wisp.Framework.Http;
using Wisp.Framework.Middleware.Auth;
using Wisp.Framework.Middleware.Sessions;
using Wisp.Framework.Views;

namespace Wisp.Demo.Controllers;

[Controller]
public class IndexController(IAuthenticator authenticator, FlashService flashService) : ControllerBase
{
    [Route("/")]
    public async Task<ViewResult> GetIndex(IHttpContextAccessor accessor)
    {
        var context = await accessor.HttpContext;

        var session = context.Session;

        var error = context.Request.QueryParams.GetValueOrDefault("error");

        return View("index", new { session = session?.Id ?? "none", error });
    }

    [Route("/private")]
    [Authorize]
    public Task<ViewResult> GetPrivate()
    {
        return Task.FromResult(View("private"));
    }

    [Route("/unauthorized")]
    public Task<ViewResult> GetUnauthorized()
    {
        return Task.FromResult(View("unauthorized"));
    }

    [Route("/simple-result")]
    public Task<ResultBox<string>> SimpleResultDemo()
    {
        return Task.FromResult(new ResultBox<string>("Hello World"));
    }

    [Route("/serialized-result")]
    public Task<ResultBox<object>> SerializedResultDemo()
    {
        return Task.FromResult(new ResultBox<object>(new { Hello = "World" }));
    }

}