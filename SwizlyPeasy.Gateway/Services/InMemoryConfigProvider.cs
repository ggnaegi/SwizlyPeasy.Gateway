using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Options;
using SwizlyPeasy.Common.Dtos;
using Yarp.ReverseProxy.Configuration;

namespace SwizlyPeasy.Gateway.Services
{
    public class InMemoryConfigProvider : IProxyConfigProvider, IHostedService, IDisposable
    {
        private volatile InMemoryConfig? _inMemoryConfig;
        private Timer _timer;
        private readonly IClusterConfigService _clusterConfigService;
        private readonly IRoutesConfigService _routesConfigService;
        private readonly IOptions<ServiceDiscoveryConfig> _serviceDiscoveryConfig;
        private readonly object _lockObject = new();

        public InMemoryConfigProvider(IClusterConfigService clusterConfigService, IRoutesConfigService routesConfigService, IOptions<ServiceDiscoveryConfig> serviceDiscoveryConfig)
        {
            _clusterConfigService = clusterConfigService ?? throw new ArgumentNullException(nameof(clusterConfigService));
            _routesConfigService = routesConfigService ?? throw new ArgumentNullException(nameof(routesConfigService));
            _serviceDiscoveryConfig = serviceDiscoveryConfig ?? throw new ArgumentNullException(nameof(serviceDiscoveryConfig));
            UpdateConfig(null);
        }

        public IProxyConfig GetConfig() => _inMemoryConfig;

        private void UpdateConfig(object? state)
        {
            lock (_lockObject)
            {
                if (_inMemoryConfig == null)
                {
                    var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, @"routes.config.json");
                    var currentConfig = File.ReadAllText(path);
                    Debug.Assert(currentConfig != null, nameof(currentConfig) + " != null");
                    _routesConfigService.LoadRoutes(_serviceDiscoveryConfig.Value.KeyValueStoreKey, currentConfig).GetAwaiter().GetResult();
                }

                var routes = _routesConfigService.GetRoutes(_serviceDiscoveryConfig.Value.KeyValueStoreKey).Result;
                var clusters = _clusterConfigService.RetrieveClustersConfig().Result;

                var oldConfig = _inMemoryConfig;
                _inMemoryConfig = new InMemoryConfig(routes, clusters);
                oldConfig?.SignalChange();
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(UpdateConfig, null, TimeSpan.Zero, TimeSpan.FromSeconds(_serviceDiscoveryConfig.Value.RefreshIntervalInSeconds));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
