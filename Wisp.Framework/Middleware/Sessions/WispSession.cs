// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

namespace Wisp.Framework.Middleware.Sessions;

public class WispSession : ISession
{
    private readonly Dictionary<string, object> _store = new();
    
    public string Id { get; set; }
    
    public T? Get<T>(string key)
    {
        return _store.TryGetValue(key, out var value) ? (T)value : default(T);
    }

    public void Set<T>(string key, T value)
    {
        _store.Add(key, value);
    }

    public void Remove(string key)
    {
        _store.Remove(key);
    }
}