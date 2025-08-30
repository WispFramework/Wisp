namespace Wisp.Extensions.Identity.OpenId.Data.Client;

public enum ErrorType
{
    HttpError = 1,
    
    JsonParseError = 100,
    
    Unknown = 99_9999
}