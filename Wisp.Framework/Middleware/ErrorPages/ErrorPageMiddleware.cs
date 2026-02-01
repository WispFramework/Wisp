// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Wisp.Framework.Extensions;
using Wisp.Framework.Http;
using Wisp.Framework.Views;

namespace Wisp.Framework.Middleware.ErrorPages;

public class ErrorPageMiddleware(ErrorPagesConfig config, TemplateRenderer tr, IHttpContextAccessor contextAccessor) : HttpMiddleware
{
    public const string ExtraDataKey = "__errorPageMiddleware_data";

    public override async Task OnRequestHandled()
    {
        var context = contextAccessor.HttpContext!;
        
        if (context.ExtraData.TryGetValue(ExtraDataKey, out var errorData) && errorData is ErrorPageData data)
        {
            var template = data.StatusCode switch
            {
                404 => config.NotFoundTemplate,
                401 or 403 => config.UnauthorizedTemplate,
                500 => config.ServerErrorTemplate,
                _ => config.ServerErrorTemplate
            };

            var model = new
            {
                StatusCode = data.StatusCode,
                FriendlyMessage = data.FriendlyMessage,
                DeveloperMessage = data.DeveloperMessage,
                Exception = data.Exception
            };

            var rendered = await tr.Render(template, model, context);

            context.Response.StatusCode = data.StatusCode;
            context.Response.ContentType = "text/html";

            var bodyStream = new MemoryStream(rendered.AsUtf8Bytes());
            bodyStream.Position = 0;
            context.Response.Body = bodyStream;
            context.IsHandled = true;
        }
    }
}