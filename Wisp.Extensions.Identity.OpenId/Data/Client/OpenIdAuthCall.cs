namespace Wisp.Extensions.Identity.OpenId.Data.Client;

public class OpenIdAuthCall
{
    public required string AuthUrl { get; set; }
    
    public required string Verifier { get; set; }
    
    public required string VerifierHash { get; set; }
    
    public required string Nonce { get; set; }
}