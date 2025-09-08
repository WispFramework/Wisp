namespace Wisp.Framework.Middleware.ErrorPages;

public class ErrorPageData
{
    public int StatusCode { get; set; }

    public string FriendlyMessage { get; set; } = "";

    public string DeveloperMessage { get; set; } = "";

    public Exception? Exception { get; set; }
}