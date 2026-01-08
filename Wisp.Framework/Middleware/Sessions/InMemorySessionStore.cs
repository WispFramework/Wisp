// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using System.Collections.Concurrent;

namespace Wisp.Framework.Middleware.Sessions;

public class InMemorySessionStore : ISessionStore
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, object?>> _store = new();
    
    public Task<T?> GetAsync<T>(string sessionId, string key)
    {
        if (!_store.TryGetValue(sessionId, out var session)) return Task.FromResult<T?>(default);
        if (session.TryGetValue(key, out var value) && value is T ret) return Task.FromResult<T?>(ret);
        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string sessionId, string key, T value)
    {
        if (!_store.TryGetValue(sessionId, out var session)) _store[sessionId] = [];
        _store[sessionId][key] = value;

        return Task.CompletedTask;
    }

    public Task ClearAsync(string sessionId, string key)
    {
        if (_store.TryGetValue(sessionId, out var session) && session.ContainsKey(key))
        {
            session[key] = null;
        }

        return Task.CompletedTask;
    }
}