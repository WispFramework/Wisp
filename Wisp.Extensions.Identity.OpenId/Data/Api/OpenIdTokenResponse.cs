using System.Text.Json.Serialization;

namespace Wisp.Extensions.Identity.OpenId.Data.Api;

public class OpenIdTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = "";
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    [JsonPropertyName("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }
    
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = "";
    
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = "";
    
    [JsonPropertyName("id_token")]
    public string IdToken { get; set; } = "";
    
    [JsonPropertyName("not_before_policy")]
    public int NotBeforePolicy { get; set; }
    
    [JsonPropertyName("session_state")]
    public string SessionState { get; set; } = "";
    
    [JsonPropertyName("scope")]
    public string Scope { get; set; } = "";

    [JsonIgnore]
    public DateTime TokenValidFrom { get; set; } = DateTime.UtcNow;
}