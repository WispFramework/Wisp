// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Middleware.Sessions;

/// <summary>
/// Session store interface
/// </summary>
public interface ISessionStore
{

    /// <summary>
    /// Get data for a session
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T?> GetAsync<T>(string sessionId, string key);
    
    /// <summary>
    /// Set data for a session
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task SetAsync<T>(string sessionId, string key, T value);
    
    /// <summary>
    /// Clear data for a session
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    Task ClearAsync(string sessionId, string key);
}