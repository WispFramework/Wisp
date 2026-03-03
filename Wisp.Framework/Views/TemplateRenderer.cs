// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Fluid;
using Fluid.ViewEngine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Wisp.Framework.Http;
using Wisp.Framework.Middleware;
using Wisp.Framework.Middleware.Auth;
using Wisp.Framework.Middleware.Sessions;
using File = System.IO.File;

namespace Wisp.Framework.Views;

public class TemplateRenderer
{

    private readonly IAuthenticator? _authenticator;
    private readonly FlashService? _flashService;

    private readonly IMiddlewareDataInjector _middlewareDataInjector;
    private readonly List<IHttpMiddleware> _middlewares;

    private readonly FluidViewEngineOptions _viewOptions = new()
    {
        ViewsFileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Templates")),
        PartialsFileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Templates")),
    };

    private readonly FluidViewRenderer _renderer;
    
    public TemplateRenderer(IServiceProvider serviceProvider, IMiddlewareDataInjector dataInjector, List<IHttpMiddleware> middlewares, List<(string Name, FilterDelegate Delegate)>? filters = null)
    {
        _authenticator = serviceProvider.GetService<IAuthenticator?>();
        _flashService = serviceProvider.GetService<FlashService?>();
        _middlewareDataInjector = dataInjector;
        _middlewares = middlewares;

        _viewOptions.TemplateOptions.MemberAccessStrategy = UnsafeMemberAccessStrategy.Instance;
        _viewOptions.TemplateOptions.MemberAccessStrategy.MemberNameStrategy = MemberNameStrategies.RenameSnakeCase;
        _viewOptions.TemplateOptions.FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Templates"));
        
        _viewOptions.TemplateOptions.Filters.AddFilter("agoDate", FluidExtensions.DateToAgo);
        _viewOptions.TemplateOptions.Filters.AddFilter("toJson", FluidExtensions.ToJson);

        foreach (var filter in filters ?? [])
        {
            _viewOptions.TemplateOptions.Filters.AddFilter(filter.Name, filter.Delegate);
        }

        _viewOptions.TemplateOptions.ValueConverters.Add((v) => v is Enum e ? $"{e}" : null);
        
        _viewOptions.Parser = new FluidViewParser(new FluidParserOptions { AllowFunctions = true, AllowParentheses = true});
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
                viewModel.CurrentUserRoles = user.Roles;
                viewModel.CurrentUserId = user.Id;
            }
        }

        if (_flashService is not null)
        {
            var flashes = _flashService.GetAllAndDelete();
            if(flashes is not null) viewModel.FlashMessages = flashes;
        }

        viewModel.CurrentRoute = context.Request.Path;

        viewModel.Middleware = await _middlewareDataInjector.GetData();

        var templateRelativePath = $"{template}.liquid";
        var templateAbsolutePath = Path.GetFullPath(Path.Combine("Templates", templateRelativePath));
        
        if(!File.Exists(templateAbsolutePath)) return $"template {templateAbsolutePath} not found";

        foreach (var mw in _middlewares)
        {
            await mw.OnTemplateRendering(viewModel);
        }
        
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