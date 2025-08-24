using System.Net;
using System.Text;
using Wisp.Tests.Fixtures;
using System.Text.Json;
using Xunit.Abstractions;

namespace Wisp.Tests.Cases.Integration;

[Collection("Server collection")]
public class ControllerInjectionTest(ServerFixture serverFixture)
{
    private HttpClient client = new();

    [Fact]
    public async Task TestControllerInjection()
    {
        var bodyObject = new { Hello = "World" };
        var bodyJson = JsonSerializer.Serialize(bodyObject);
        
        var content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
        
        var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:22222/democontroller")
        {
            Content = content
        };
        
        request.Headers.Add("DemoHeader", "Hello-World");
        request.Headers.Add("Cookie", "DemoCookie=HelloWorld");
        
        var response = await client.SendAsync(request);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();

        Assert.NotNull(body);

        var parsed = JsonSerializer.Deserialize<JsonElement>(body);
        
        Assert.Equal("World", parsed.GetProperty("Body").GetProperty("Hello").GetString());
        Assert.Equal("Hello-World", parsed.GetProperty("Header").GetString());
        Assert.Equal("HelloWorld", parsed.GetProperty("Cookie").GetString());
    }
}