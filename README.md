# Wisp Framework

[![NuGet Version](https://img.shields.io/nuget/v/Wisp.Framework.Core)](https://www.nuget.org/packages/Wisp.Framework.Core)
[![GitHub Release](https://img.shields.io/github/v/release/WispFramework/Wisp)](https://github.com/WispFramework/Wisp/releases)
![Static Badge](https://img.shields.io/badge/license-MIT%2FApache2.0-green)
![GitHub Issues or Pull Requests](https://img.shields.io/github/issues/WispFramework/Wisp)
![GitHub Issues or Pull Requests](https://img.shields.io/github/issues-pr/WispFramework/Wisp)

> **GitHub/Codeberg Update:** After much deliberation, it has been decided to migrate development of Wisp back to GitHub
> for the time being. This is not to say that Codeberg isn't great. It is. It's just that for a small project like Wisp,
> the mainstream-ness of GitHub is a benefit.
> The [CodeBerg Repo](https://codeberg.org/WispFramework/Wisp) continues to exist and will act as a mirror for GitHub now.
> Please use GitHub for submitting [Issues](https://github.com/WispFramework/Wisp/issues) and 
> [Pull Requests](https://github.com/WispFramework/Wisp/pulls).

Wisp is an open-source, cross-platform, embeddable web framework for building small, lightweight
applications. It does not depend on any part of the ASP.NET Core SDK, runs on any platform supported
by .NET Core and works with any .NET language. It's specifically designed for embedding in existing
applications so it's fully self-contained and has no special environment requirements.

[Learn more about Wisp](https://wispframework.github.io/Wisp/)

> **Warning!** Wisp is in very early stages of development and production use is not recommended. Every release, even minor, might
> introduce significant breaking changes.

## Get Started

Follow the [Getting Started](https://wispframework.github.io/Wisp/Wisp_Framework/getting-started/) guide.

All you need to start developing with Wisp is the .NET Core SDK 10+ and a text editor.

## Minimal Example

```
dotnet new console
dotnet add pacakge Wisp.Framework.Core
```

`Program.cs`:
```csharp
var hostBuilder = new WispHostBuilder();
var appBuilder = hostBuilder.Build();

appBuilder.ConfigureRoutes(r => {
  r.Get("/", ctx => {
    ctx.Response.Body.Write("Hello World"u8);
  });
});

var app = appBuilder.Build();
await app.RunAsync();
```

## License

This project is dual-licensed under either:

- [Apache 2.0](LICENSE-APACHE)
- [MIT](LICENSE-MIT)

at your option.
