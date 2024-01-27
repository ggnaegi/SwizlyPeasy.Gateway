using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace SwizlyPeasy.Gateway.Services;

/// <summary>
///     Custom proxy config
/// </summary>
public class InMemoryConfig : IProxyConfig, IDisposable
{
    private readonly CancellationTokenSource _cts = new();

    // To detect redundant calls
    private bool _disposedValue;

    public InMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
    {
        Routes = routes;
        Clusters = clusters;
        ChangeToken = new CancellationChangeToken(_cts.Token);
    }

    public IReadOnlyList<RouteConfig> Routes { get; }
    public IReadOnlyList<ClusterConfig> Clusters { get; }
    public IChangeToken ChangeToken { get; }

    internal void SignalChange()
    {
        _cts.Cancel();
    }

    // Public implementation of Dispose pattern callable by consumers.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            _cts.Dispose();
        }

        _disposedValue = true;
    }
}