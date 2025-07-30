// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Fluid;
using Fluid.ViewEngine;
using Microsoft.Extensions.FileProviders;

namespace Wisp.Framework.Views;

public class TemplateRenderer
{
    public static Lazy<TemplateRenderer> Instance = new Lazy<TemplateRenderer>(() => new TemplateRenderer());

    private readonly FluidViewEngineOptions _viewOptions = new FluidViewEngineOptions
    {
        ViewsFileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Templates")),
        PartialsFileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Templates")),
    };

    private readonly FluidViewRenderer _renderer;

    private TemplateRenderer()
    {
        _viewOptions.TemplateOptions.MemberAccessStrategy = UnsafeMemberAccessStrategy.Instance;
        _viewOptions.TemplateOptions.MemberAccessStrategy.MemberNameStrategy = MemberNameStrategies.RenameSnakeCase;
        _viewOptions.TemplateOptions.FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Templates"));
        _renderer = new(_viewOptions);
    }

    public async Task<string> Render(string template, object model)
    {
        var ctx = new TemplateContext(model, _viewOptions.TemplateOptions);
        await using var sw = new StringWriter();
        await _renderer.RenderViewAsync(sw, template, ctx);
        var str = sw.ToString();
        return str;
    }
}