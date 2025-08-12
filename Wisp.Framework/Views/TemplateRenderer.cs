// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Fluid;
using Fluid.ViewEngine;
using Microsoft.Extensions.FileProviders;
using Wisp.Framework.Http;
using Wisp.Framework.Middleware.Auth;
using Wisp.Framework.Middleware.Sessions;

namespace Wisp.Framework.Views;

public class TemplateRenderer
{

    private readonly IAuthenticator? _authenticator;
    private readonly FlashService? _flashService;

    private readonly FluidViewEngineOptions _viewOptions = new()
    {
        ViewsFileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Templates")),
        PartialsFileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Templates")),
    };

    private readonly FluidViewRenderer _renderer;

    public TemplateRenderer(IAuthenticator? authenticator, FlashService? flashService)
    {
        _authenticator = authenticator;
        _flashService = flashService;

        _viewOptions.TemplateOptions.MemberAccessStrategy = UnsafeMemberAccessStrategy.Instance;
        _viewOptions.TemplateOptions.MemberAccessStrategy.MemberNameStrategy = MemberNameStrategies.RenameSnakeCase;
        _viewOptions.TemplateOptions.FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Templates"));
        _renderer = new(_viewOptions);
    }

    public async Task<string> Render(string template, object model, IHttpContext context)
    {
        var viewModel = new ViewModel { Model = model };

        if (_authenticator != null)
        {
            var user = await _authenticator.GetUser();
            if (user != null)
            {
                viewModel.UserLoggedIn = true;
                viewModel.CurrentUserName = user.Username;
                viewModel.CurrentUserRole = user.Role;
            }
        }

        if (_flashService is not null)
        {
            var flashes = await _flashService.GetAllAndDelete();
            if(flashes is not null) viewModel.FlashMessages = flashes;
        }

        var templateRelativePath = $"{template}.liquid";
        var templateAbsolutePath = Path.GetFullPath(Path.Combine("Templates", templateRelativePath));
        
        if(!File.Exists(templateAbsolutePath)) return $"template {templateAbsolutePath} not found";

        try
        {
            var ctx = new TemplateContext(viewModel, _viewOptions.TemplateOptions);
            await using var sw = new StringWriter();
            await _renderer.RenderViewAsync(sw, template + ".liquid", ctx);
            var str = sw.ToString();
            return str;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}