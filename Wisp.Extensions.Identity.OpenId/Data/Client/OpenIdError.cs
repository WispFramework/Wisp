namespace Wisp.Extensions.Identity.OpenId.Data.Client;

public class OpenIdError
{
    
    public string ErrorMessage { get; set; } = "";

    public ErrorType ErrorType { get; set; } = ErrorType.Unknown;

    public int HttpStatusCode { get; set; } = 0;
    
    public string? Context { get; set; }
    
    public Exception? Exception { get; set; }
}