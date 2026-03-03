using Wisp.Framework.Controllers;
using Wisp.Framework.Views;

namespace Mvc.Controllers;

[Controller]
public class HomeController : ControllerBase
{
    [Route("")]
    public ViewResult GetIndex() => View("index");
}