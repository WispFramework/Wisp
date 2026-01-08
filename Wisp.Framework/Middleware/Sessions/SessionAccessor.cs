// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Microsoft.Extensions.Logging;
using Wisp.Framework.Extensions;
using Wisp.Framework.Http;

namespace Wisp.Framework.Middleware.Sessions;

public class SessionAccessor(ISessionStore store, IHttpContextAccessor accessor, ILogger<SessionAccessor> log) : ISessionAccessor
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
        if (context is null)
        {
            return Task.FromResult<string?>(null);
        }
        
        var sessionId = context.Request.Cookies.GetOrDefaultIgnoreCaseReadonly("WISP_SESSION");
        if (sessionId is not null)
        {
            log.LogDebug("Session ID is {Id}", sessionId);
            return Task.FromResult<string?>(sessionId);
        }

        log.LogDebug("Session ID was not found, creating a new one");
        var newId = Guid.NewGuid();
        context.Response.Cookies["WISP_SESSION"] = newId.ToString();
        return Task.FromResult<string?>(newId.ToString());
    }
}