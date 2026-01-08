// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Wisp.Framework.Http;

namespace Wisp.Framework.Middleware.Sessions;

public class SessionAccessor(ISessionStore store, IHttpContextAccessor accessor) : ISessionAccessor
{
    public async Task<T?> GetAsync<T>(string key)
    {
        var sessionId = await GetSessionId();
        if (sessionId is null) return default;
        
        return await store.GetAsync<T>(sessionId, key);
    }

    public async Task SetAsync<T>(string key, T value)
    {
        var sessionId = await GetSessionId();
        if (sessionId is null) return;
        await store.SetAsync(sessionId, key, value);
    }

    public async Task ClearAsync(string key)
    {
        var sessionId = await GetSessionId();
        if (sessionId is null) return;
        await store.ClearAsync(sessionId, key);
    }

    public Task<string?> GetSessionId()
    {
        var context = accessor.HttpContext;
        return Task.FromResult(context?.Request.Cookies.GetValueOrDefault("WISP_SESSION"));
    }
}