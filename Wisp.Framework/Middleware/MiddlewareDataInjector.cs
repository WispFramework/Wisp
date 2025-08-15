
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