// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Views;

public class TemplateView : IView
{
    private bool _isRedirect = false;

    private string? _redirectUri;

    public string TemplateName { get; set; }
    
    public bool IsRedirect => _isRedirect;

    public string? RedirectUri
    {
        get => _redirectUri;
        set 
        {
            _redirectUri = value;
            _isRedirect = true;
        }
    }

    public object Model { get; set; } = new { };

    public TemplateView(string name, object? model = null)
    {
        TemplateName = name;
        Model = model ?? new { };
    }

    public TemplateView(string uri)
    {
        RedirectUri = uri;
    }
}