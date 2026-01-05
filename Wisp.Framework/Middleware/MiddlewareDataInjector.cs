// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.Collections.Concurrent;

namespace Wisp.Framework.Middleware;

public class MiddlewareDataInjector : IMiddlewareDataInjector
{
    private readonly AsyncLocal<ConcurrentDictionary<string, object?>> _store = new();

    public Task<Dictionary<string, object?>> GetData()
    {
        _store.Value ??= new();
        return Task.FromResult(_store.Value.ToDictionary());
    }

    public Task Inject(string key, object? item)
    {
        _store.Value ??= new();
        _store.Value[key] = item;

        return Task.CompletedTask;
    }
}