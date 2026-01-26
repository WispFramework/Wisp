// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Wisp.Tests.Fixtures;

namespace Wisp.Tests.Cases.Integration;

[Collection("Server collection")]
public class BasicHttp(ServerFixture serverFixture)
{
    private HttpClient client = new HttpClient();

    [Fact]
    public async Task TestGetIndex()
    {
        var response = await client.GetAsync("http://localhost:22222/");

        Assert.True(response.IsSuccessStatusCode);
    }
}