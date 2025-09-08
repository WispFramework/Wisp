using Microsoft.Extensions.Logging;

namespace Wisp.Framework.Hosting;

public class BackgroundServiceManager : IDisposable
{
    private readonly IEnumerable<IBackgroundService> _services;
    private readonly ILogger<BackgroundServiceManager> _log;
    private CancellationTokenSource _cts = new();

    private readonly List<Task> _tasks = new();

    public BackgroundServiceManager(IEnumerable<IBackgroundService> services, ILogger<BackgroundServiceManager> logger)
    {
        _services = services;
        _log = logger;
    }

    public void Dispose()
    {
        _cts.Dispose();
    }

    public Task RunAsync(CancellationToken? cancel = default)
    {
        if (cancel is not null) _cts = CancellationTokenSource.CreateLinkedTokenSource(cancel.Value);

        _log.LogInformation("starting {Count} background services", _services is ICollection<IBackgroundService> c ? c.Count : -1);

        foreach (var service in _services)
        {
            _log.LogInformation("starting service {Name}", service.GetType().Name);
            _tasks.Add(Task.Run(() => service.RunAsync(_cts.Token)));
        }

        return Task.CompletedTask;
    }

    public async Task Stop()
    {
        _log.LogInformation("stopping background services");

        _cts.Cancel();

        var timeout = TimeSpan.FromSeconds(10);
        var delay = Task.Delay(timeout);
        var compound = Task.WhenAll(_tasks);

        var winner = await Task.WhenAny(compound, delay).ConfigureAwait(false);

        if (winner == compound)
        {
            try
            {
                await compound.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "one or more background services threw an exception");
                throw;
            }
        }
        else
        {
            _log.LogError("one or more background tasks did not exit in time and will be killed");
            await Task.Delay(5);
            Environment.Exit(1);
        }
    }
}