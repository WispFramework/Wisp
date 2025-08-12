using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Wisp.Framework.Middleware.Sessions;

namespace Wisp.Extensions.RedisSession;

public class RedisSessionStore(RedisSessionStoreConfig config, ILogger<RedisSessionStore> log) : ISessionStore
{
    private ConnectionMultiplexer? _connection;

    public async Task InitializeAsync()
    {
        try
        {
            _connection = await ConnectionMultiplexer.ConnectAsync(config.ConnectionString);
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<ISession?> GetAsync(string sessionId)
    {
        if(_connection is null) throw new NullReferenceException("the Redis connection has not been initialized");
        
        //var db = _connection.GetDatabase();
        //string? session = await db.StringGetAsync($"wisp_session_{sessionId}").ConfigureAwait(false);
        //if (session is null) return null;
        
        try
        {
          //  var parsed = JsonSerializer.Deserialize<WispSession>(session);
            //return parsed;
        }
        catch (NotSupportedException)
        {
            log.LogError("can't load session, it contains data that's not JSON serializable");
            return null;
        }

        return new WispSession { Id = "cocks" };
    }

    public async Task<ISession> CreateAsync()
    {
        if(_connection is null) throw new NullReferenceException("the Redis connection has not been initialized");
        
        var id = Guid.NewGuid().ToString();
        var session = new WispSession
        {
            Id = id
        };
        
        //var json = JsonSerializer.Serialize(session);    
        
        //var db = _connection.GetDatabase();
        //await db.StringSetAsync($"wisp_session_{id}", json).ConfigureAwait(false);
        
        return session;
    }

    public async Task StoreAsync(string sessionId, ISession session)
    {
        if(_connection is null) throw new NullReferenceException("the Redis connection has not been initialized");
        
        //var db = _connection.GetDatabase();
        
        try
        {
          //  var json = JsonSerializer.Serialize(session);
            //await db.StringSetAsync($"wisp_session_{sessionId}", json).ConfigureAwait(false);
        }
        catch (NotSupportedException)
        {
            log.LogError("can't store session, it contains data that's not JSON serializable");
        }
    }
}