using Microsoft.Extensions.Options;
using SwizlyPeasy.Common.Dtos;
using SwizlyPeasy.Common.Dtos.Status;
using SwizlyPeasy.Consul.Health;
using Yarp.ReverseProxy.Configuration;

namespace SwizlyPeasy.Gateway.Services;

public class StatusService : IStatusService
{
    private readonly IClusterConfigService _clusterConfigService;
    private readonly IHealthCheckService _healthCheckService;
    private readonly IOptions<ServiceDiscoveryConfig> _serviceDiscoveryConfig;

    public StatusService(IClusterConfigService clusterConfigService, IHealthCheckService healthCheckService,
        IOptions<ServiceDiscoveryConfig> serviceDiscoveryConfig)
    {
        _clusterConfigService = clusterConfigService ?? throw new ArgumentNullException(nameof(clusterConfigService));
        _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));
        _serviceDiscoveryConfig =
            serviceDiscoveryConfig ?? throw new ArgumentNullException(nameof(serviceDiscoveryConfig));
    }

    public async Task<StatusDto> GetGatewayStatus()
    {
        var clusterConfig = await _clusterConfigService.RetrieveClustersConfig();
        var status = new StatusDto
        {
            ServiceDiscoveryAddress = _serviceDiscoveryConfig.Value.ServiceDiscoveryAddress.OriginalString,
            Clusters = new List<ClusterStatusDto>()
        };

        if (!clusterConfig.Any()) return status;

        foreach (var cluster in clusterConfig) status.Clusters.Add(await GetClusterStatus(cluster));

        return status;
    }

    private async Task<ClusterStatusDto> GetClusterStatus(ClusterConfig cluster)
    {
        var clusterStatus = new ClusterStatusDto
        {
            ClusterId = cluster.ClusterId,
            Destinations = new List<DestinationStatusDto>()
        };

        if (cluster.Destinations == null || !cluster.Destinations.Any()) return clusterStatus;

        foreach (var serviceId in cluster.Destinations.Keys)
            clusterStatus.Destinations.Add(await GetDestinationStatus(serviceId,
                cluster.Destinations[serviceId].Address));

        clusterStatus.Healthy = clusterStatus.Destinations.All(x => x.Healthy);

        return clusterStatus;
    }

    private async Task<DestinationStatusDto> GetDestinationStatus(string serviceId, string address)
    {
        var healthy = await _healthCheckService.IsServiceHealthy(serviceId);

        return new DestinationStatusDto
        {
            Id = serviceId,
            Address = address,
            Healthy = healthy
        };
    }
}