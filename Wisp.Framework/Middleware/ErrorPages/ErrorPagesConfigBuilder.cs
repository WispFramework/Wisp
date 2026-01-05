// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

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