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

    private Uri? _redirectUri;
    
    public bool IsRedirect => _isRedirect;

    public Uri? RedirectUri
    {
        get => _redirectUri;
        set 
        {
            _redirectUri = value;
            _isRedirect = true;
        }
    }

    private string _templateName;

    public object Model { get; set; } = new { };

    public TemplateView(string name, object? model = null)
    {
        _templateName = name;
        Model = model ?? new { };
    }

    public TemplateView(Uri uri)
    {
        RedirectUri = uri;
    }

    public async Task<(string HtmlOrAddress, bool IsRedirect)> Render()
    {
        if (_isRedirect) return (_redirectUri?.ToString() ?? "/", true);
        
        var templateFileName = Path.Combine("Templates", Path.ChangeExtension(_templateName, ".liquid"));
        // if (!File.Exists(templateFileName)) return ($"Template '{templateFileName}' not found", false);
        // var template = File.ReadAllText(templateFileName);
        
        return (await TemplateRenderer.Instance.Value.Render(_templateName + ".liquid", Model), false);
    }
}