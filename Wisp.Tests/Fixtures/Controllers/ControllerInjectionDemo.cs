using Wisp.Framework.Controllers;

namespace Wisp.Tests.Fixtures.Controllers;

[Controller]
public class ControllerInjectionDemo : ControllerBase
{
    [Route("/democontroller", "POST")]
    public async Task<IResultBox<object>> GetDemo([FromBody] object body, [Header] string demoHeader, [Cookie] string demoCookie)
    {
        return new ResultBox<object>(new
        {
            Body = body,
            Cookie = demoCookie,
            Header = demoHeader
        });
    }
}