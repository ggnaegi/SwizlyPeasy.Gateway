using Microsoft.Extensions.Options;
using SwizlyPeasy.Common.Dtos;
using SwizlyPeasy.Consul.Agents;
using Yarp.ReverseProxy.Configuration;

namespace SwizlyPeasy.Gateway.Services;

public class ClusterConfigService(IRetrieveAgentsService agentsService, IOptions<ServiceDiscoveryConfig> config)
    : IClusterConfigService
{
    private readonly IRetrieveAgentsService _agentsService = agentsService ?? throw new ArgumentNullException(nameof(agentsService));
    private readonly IOptions<ServiceDiscoveryConfig> _config = config ?? throw new ArgumentNullException(nameof(config));

    public async Task<List<ClusterConfig>> RetrieveClustersConfig()
    {
        var agentsDic = await _agentsService.RetrieveAgents();

        if (agentsDic.Count == 0)
        {
            return [];
        }

        return agentsDic.Keys
            .Select(serviceName => new ClusterConfig
            {
                ClusterId = serviceName,
                LoadBalancingPolicy = _config.Value.LoadBalancingPolicy,
                Destinations = agentsDic[serviceName]
                    .Select(x => (x.ID,
                        new DestinationConfig { Address = $"{_config.Value.Scheme}://{x.Address}:{x.Port}" }))
                    .ToDictionary(y => y.ID, y => y.Item2)
            })
            .ToList();
    }
}