# Wisp Framework

[![NuGet Version](https://img.shields.io/nuget/v/Wisp.Framework.Core)](https://www.nuget.org/packages/Wisp.Framework.Core)
[![GitHub Tag](https://img.shields.io/github/v/tag/UTF-8x/Wisp)](https://github.com/utf-8x/wisp)
![Static Badge](https://img.shields.io/badge/license-MIT%2FApache2.0-green)
[![GitHub Issues or Pull Requests](https://img.shields.io/github/issues/UTF-8x/Wisp)](https://github.com/utf-8x/wisp/issues)
[![GitHub Issues or Pull Requests](https://img.shields.io/github/issues-pr/UTF-8x/Wisp)](https://github.com/utf-8x/wisp/pulls)

> **Warning!** Wisp is in very early stages of development and production use is not recommended.

Wisp is an open-source, cross-platform, embeddable web framework for building small, lightweight
applications. It does not depend on any part of the ASP.NET Core SDK, runs on any platform supported
by .NET Core and works with any .NET language. It's specifically designed for embedding in existing
applications so it's fully self-contained and has no special environment requirements.

[Learn more about Wisp](https://wisp.jakubsycha.com/)

## Get Started

Follow the [Getting Started](https://wisp.jakubsycha.com/docs/1-getting-started) guide.

All you need to start developing with Wisp is the .NET Core SDK 9+ and a text editor.

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
