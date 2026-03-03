# Welcome to Wisp Framework

## Not big on introductions?

**[Jump straight to Getting Started](/Wisp_Framework/getting-started)**

## What is Wisp?

Wisp is a lightweight web framework for .NET, designed with one primary goal: to create a web framework that only requires
the base .NET Core runtime and has zero dependencies on the ASP.NET Core Web SDK. It focuses on simplicity, explicitness,
and minimal dependencies while still following established patterns that .NET developers are familiar with, such as
dependency injection, configuration, and structured logging.

The design philosophy behind Wisp is pragmatic:

- **Minimal runtime dependencies** – It avoids heavy abstractions that often make small projects depend on large
  frameworks.
- **Adaptable backend** – The HTTP server itself is pluggable; while Wisp ships with a
  [NetCoreServer](https://github.com/chronoxor/NetCoreServer) backend by default, the core framework is designed to
  allow swapping the underlying transport if project needs change.
- **Familiar development model** – Despite being lightweight, Wisp includes features developers expect from modern
  frameworks: routing, controller support, template rendering, and configuration binding.
- **Fully Embeddable** - Wisp can be embedded into any existing .NET Core application without hijacking your application
  lifecycle and without having to add the ASP .NET Core Web SDK dependency.

### When to Use Wisp

Wisp is a good fit when you need a lightweight but fully featured Web framework and don't want the overhead of the full
ASP.NET Core SDK, for example:

- Simple, lightweight Web Applications and APIs
- Embedding a control or monitoring API into a desktop or service application
- Quickly exposing endpoints for local tools or development utilities
- Building small standalone services where using the full ASP.NET Core SDK would be overkill
- Creating micro-APIs using the new [File-based apps functionality](https://learn.microsoft.com/en-us/dotnet/core/sdk/file-based-apps)
  from .NET 10.

It is **not** intended as a drop-in replacement for ASP.NET Core for large, production-scale web applications. Instead,
Wisp is intended to be small, understandable, and easily embeddable.

### Design Goals
1. **Lightweight:** Minimal external dependencies and no requirement for the ASP.NET Core SDK.
2. **Explicit APIs:** No implicit discovery or convention-over-configuration by default[^1]. You define what exists,
   and Wisp executes it.
3. **Extensible:** Routing, controllers, and even the HTTP backend can be replaced or extended as needed.
4. **Approachable:** If you know basic C# and have worked with ASP.NET before, you should feel at home.

### How lightweight?

Wisp is intentionally small and avoids pulling in large frameworks by default.  

Here is the full list of its direct runtime dependencies, there are exactly 10 of them:

```
Fluid.Core
Fluid.ViewEngine
HttpMultipartParser
Microsoft.Extensions.Configuration
Microsoft.Extensions.Configuration.Binder
Microsoft.Extensions.Configuration.Json
Microsoft.Extensions.DependencyInjection
Microsoft.Extensions.Logging.Console
Microsoft.Extensions.Options
NetCoreServer
```

There are of course some transitive dependencies the direct dependencies pull in. The total count is around 25. For
reference, a basic ASP.NET Core app bootstrapped with `dotnet new webapps` pulls in around 150 transitive dependencies.

[^1]: This does not mean Wisp doesn't have powerful auto-discovery and autoconfiguration features. It simply means that
you have to enable these features manually so you don't get surprised by unexpected auto-magic.