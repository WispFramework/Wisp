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
    /// Get a session object by ID
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    Task<ISession?> GetAsync(string sessionId);
    
    /// <summary>
    /// Create a new session object
    /// </summary>
    /// <returns></returns>
    Task<ISession> CreateAsync();
    
    /// <summary>
    /// Store a session object by ID
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="session"></param>
    /// <returns></returns>
    Task StoreAsync(string sessionId, ISession session);
}