# Wisp Framework

[![NuGet Version](https://img.shields.io/nuget/v/Wisp.Framework.Core)](https://www.nuget.org/packages/Wisp.Framework.Core)
[![Gitea Release](https://img.shields.io/gitea/v/release/WispFramework/Wisp?gitea_url=https%3A%2F%2Fcodeberg.org)](https://codeberg.org/WispFramework/Wisp/releases)
![Static Badge](https://img.shields.io/badge/license-MIT%2FApache2.0-green)
[![Gitea Issues](https://img.shields.io/gitea/issues/open/WispFramework/Wisp?gitea_url=https%3A%2F%2Fcodeberg.org)](https://codeberg.org/WispFramework/Wisp/issues)
[![Gitea Pull Requests](https://img.shields.io/gitea/pull-requests/open/WispFramework/Wisp?gitea_url=https%3A%2F%2Fcodeberg.org)](https://codeberg.org/WispFramework/Wisp/pulls)

> **GitHub Users:** This repository is a mirror of the upstream on [CodeBerg](https://codeberg.org/WispFramework/Wisp). At this time,
> issues and pull requests will only be accepted in the upstream repo.

Wisp is an open-source, cross-platform, embeddable web framework for building small, lightweight
applications. It does not depend on any part of the ASP.NET Core SDK, runs on any platform supported
by .NET Core and works with any .NET language. It's specifically designed for embedding in existing
applications so it's fully self-contained and has no special environment requirements.

[Learn more about Wisp](https://wisp.jakubsycha.com/)

> **Warning!** Wisp is in very early stages of development and production use is not recommended.

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
