using System.Collections.Concurrent;

namespace Wisp.Framework.Middleware.Sessions;

public class InMemorySessionStore : ISessionStore
{
    private readonly ConcurrentDictionary<string, ISession> _store = new();
    
    public Task<ISession?> GetAsync(string sessionId)
    {
        return Task.FromResult(_store.GetValueOrDefault(sessionId));
    }

    public Task<ISession> CreateAsync()
    {
        var id = Guid.NewGuid().ToString();
        var session = new WispSession
        {
            Id = id,
        };
        _store[id] = session;
        
        return Task.FromResult<ISession>(session);
    }

    public Task StoreAsync(string sessionId, ISession session)
    {
        _store[sessionId] = session;
        
        return Task.CompletedTask;
    }
}