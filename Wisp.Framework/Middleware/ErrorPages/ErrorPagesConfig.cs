namespace Wisp.Framework.Middleware.ErrorPages;

public class ErrorPagesConfig
{
    public string NotFoundTemplate { get; set; } = "/errors/404";

    public string UnauthorizedTemplate { get; set; } = "/error/401";

    public string ServerErrorTemplate { get; set; } = "/error/500";
}