using Wisp.Framework.Controllers;
using Wisp.Framework.Middleware.Auth;
using Wisp.Framework.Views;

namespace Wisp.Demo.Controllers.Admin;

[Controller]
public class AdminIndexController : ControllerBase
{
    [Route("/admin")]
    [Authorize("admin")]
    public async Task<IView> GetIndex()
    {
        return View("admin/index");
    }
}