// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Wisp.Framework.Controllers;

namespace Wisp.Tests.Fixtures.Controllers;

[Controller]
public class ControllerInjectionDemo : ControllerBase
{
    [Route("/democontroller", "POST")]
    public async Task<IResultBox<object>> GetDemo([FromBody] object body, [FromHeader] string demoHeader, [FromCookie] string demoCookie)
    {
        return new ResultBox<object>(new
        {
            Body = body,
            Cookie = demoCookie,
            Header = demoHeader
        });
    }
}