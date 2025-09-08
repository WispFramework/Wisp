namespace Wisp.Framework.Hosting;

public interface IBackgroundService
{
    Task RunAsync(CancellationToken cancellationToken);
}