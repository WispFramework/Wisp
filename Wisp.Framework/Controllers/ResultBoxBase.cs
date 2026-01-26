using Wisp.Framework.Util;

namespace Wisp.Framework.Controllers;

public class ResultBoxBase
{
    public int StatusCode { get; set; } = HttpStatusCode.OK;
}