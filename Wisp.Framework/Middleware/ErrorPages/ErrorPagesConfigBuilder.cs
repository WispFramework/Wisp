namespace Wisp.Framework.Middleware.ErrorPages;

public class ErrorPagesConfigBuilder
{
    private ErrorPagesConfig _config = new();

    public ErrorPagesConfigBuilder WithNotFoundTemplate(string tpl)
    {
        _config.NotFoundTemplate = tpl;
        return this;
    }

    public ErrorPagesConfigBuilder WithUnauthorizedTemplate(string tpl)
    {
        _config.UnauthorizedTemplate = tpl;
        return this;
    }

    public ErrorPagesConfigBuilder WithServerErrorTemplate(string tpl)
    {
        _config.ServerErrorTemplate = tpl;
        return this;
    }

    public ErrorPagesConfig Build() => _config;
}