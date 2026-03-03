# Wisp Framework

[![NuGet Version](https://img.shields.io/nuget/v/Wisp.Framework.Core)](https://www.nuget.org/packages/Wisp.Framework.Core)
[![GitHub Release](https://img.shields.io/github/v/release/WispFramework/Wisp)](https://github.com/WispFramework/Wisp/releases)
![Static Badge](https://img.shields.io/badge/license-MIT%2FApache2.0-green)
![GitHub Issues or Pull Requests](https://img.shields.io/github/issues/WispFramework/Wisp)
![GitHub Issues or Pull Requests](https://img.shields.io/github/issues-pr/WispFramework/Wisp)

Wisp is an open-source, cross-platform, embeddable web framework for building small, lightweight
applications. It does not depend on any part of the ASP.NET Core SDK, runs on any platform supported
by .NET Core and works with any .NET language. It's specifically designed for embedding in existing
applications so it's fully self-contained and has no special environment requirements.

> **Warning!** Wisp is in very early stages of development and production use is not recommended. Every release, even minor, might
> introduce significant breaking changes.

### [Learn more about Wisp](https://wispframework.github.io/Wisp/)

### [Contribute to Wisp](https://wispframework.github.io/Wisp/Wisp_Framework/contributing)

## Get Started

Follow the [Getting Started](https://wispframework.github.io/Wisp/Wisp_Framework/getting-started/) guide.

All you need to start developing with Wisp is the .NET Core SDK 10+ and a text editor.

## Minimal Example

```
dotnet new install Wisp.Framework.Templates
dotnet new wisp.mvc
dotnet run
```

And go to [http://localhost:6969](http://localhost:6969)

## License

This project is dual-licensed under either:

- [Apache 2.0](LICENSE-APACHE)
- [MIT](LICENSE-MIT)

at your option.
