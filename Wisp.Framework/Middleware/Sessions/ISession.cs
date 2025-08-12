// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Middleware.Sessions;

public interface ISession
{
    /// <summary>
    /// Unique ID for the session
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Get a stored item
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T? Get<T>(string key);
    
    /// <summary>
    /// Store an item
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    void Set<T>(string key, T value);
    
    /// <summary>
    /// Remove a stored item
    /// </summary>
    /// <param name="key"></param>
    void Remove(string key);
}