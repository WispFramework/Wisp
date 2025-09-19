// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Wisp.Framework.Util;
using Wisp.Framework.Views;

namespace Wisp.Framework.Controllers;

/// <summary>
/// A collection of convenience features for MVC controllers
/// </summary>
public abstract class ControllerBase
{
    protected internal ViewResult View(string templateName, object? model = null)
        => new ViewResult(new TemplateView(templateName, model));

    protected internal ViewResult Redirect(string url)
        => new ViewResult(new TemplateView(url));

    protected internal IResultBox<T> Box<T>(T item) => new ResultBox<T>(item);
    
    protected internal ResultBox FromError(Error error) => new ResultBox(error);
    
    protected internal ResultBox ServerError(string message, string? description = null, Exception? exception = null)
        => new ResultBox(new Error(500, message, description, exception));

    protected internal ResultBox NotFound(string message, string? description = null, Exception? exception = null)
        => new ResultBox(new Error(404, message, description, exception));
    
    protected internal ResultBox Ok<T>(T content) => new ResultBox(content);
    
}