// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Microsoft.Extensions.DependencyInjection;

namespace Wisp.Framework.Util.ServiceDiscovery;

/// <summary>
/// Marks a service that will be automatically added to the main DI container
/// </summary>
/// <param name="lifetime"></param>
[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Singleton) : Attribute
{
    public ServiceLifetime Lifetime { get; set; } = lifetime;
}