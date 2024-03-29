﻿using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SwizlyPeasy.Common.Dtos;
using SwizlyPeasy.Common.Exceptions;
using Yarp.ReverseProxy.Configuration;

namespace SwizlyPeasy.Gateway.Services;

public class InMemoryConfigProvider(
    IClusterConfigService clusterConfigService,
    IRoutesConfigService routesConfigService,
    IOptions<ServiceDiscoveryConfig> serviceDiscoveryConfig)
    : IProxyConfigProvider, IHostedService, IDisposable
{
    private readonly IClusterConfigService _clusterConfigService = clusterConfigService ?? throw new ArgumentNullException(nameof(clusterConfigService));

    private readonly object _lockObject = new();
    private readonly IRoutesConfigService _routesConfigService = routesConfigService ?? throw new ArgumentNullException(nameof(routesConfigService));
    private readonly IOptions<ServiceDiscoveryConfig> _serviceDiscoveryConfig = serviceDiscoveryConfig ?? throw new ArgumentNullException(nameof(serviceDiscoveryConfig));

    // To detect redundant calls
    private bool _disposedValue;
    private volatile InMemoryConfig? _inMemoryConfig;
    private Timer? _timer;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Starting IHostedService,
    ///     A timer is created, updating the YARP configuration when the interval is reached.
    ///     -> ServiceDiscoveryConfig
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(UpdateConfig, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(_serviceDiscoveryConfig.Value.RefreshIntervalInSeconds));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public IProxyConfig GetConfig()
    {
        if (_inMemoryConfig == null) UpdateConfig(null);

        if (_inMemoryConfig == null) throw new InternalDomainException("IProxyConfig can't be null!", null);

        return _inMemoryConfig;
    }

    /// <summary>
    ///     Updating the YARP configuration:
    ///     If config is null, retrieving the routes from the file routes.config.json and then saving the raw data
    ///     in consul kv store. The routes are then retrieved from kv store, allowing changes on the fly.
    /// </summary>
    /// <param name="state"></param>
    private void UpdateConfig(object? state)
    {
        lock (_lockObject)
        {
            if (_inMemoryConfig == null)
            {
                var path = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
                    @"routes.config.json");
                var currentConfig = File.ReadAllText(path);
                Debug.Assert(currentConfig != null, nameof(currentConfig) + " != null");
                _routesConfigService.LoadRoutes(_serviceDiscoveryConfig.Value.KeyValueStoreKey, currentConfig)
                    .GetAwaiter().GetResult();
            }

            var routes = _routesConfigService.GetRoutes(_serviceDiscoveryConfig.Value.KeyValueStoreKey).Result;
            var clusters = _clusterConfigService.RetrieveClustersConfig().Result;

            var oldConfig = _inMemoryConfig;
            _inMemoryConfig = new InMemoryConfig(routes, clusters);
            oldConfig?.SignalChange();
        }
    }

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing) _timer?.Dispose();

        _disposedValue = true;
    }
}