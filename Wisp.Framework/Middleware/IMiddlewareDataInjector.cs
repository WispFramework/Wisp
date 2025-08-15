namespace Wisp.Framework.Middleware;

public interface IMiddlewareDataInjector
{
    Task Inject(string key, object? item);

    Task<Dictionary<string, object?>> GetData();
}