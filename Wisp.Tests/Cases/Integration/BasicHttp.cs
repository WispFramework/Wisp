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