using Consul;
using Microsoft.Extensions.Options;
using SwizlyPeasy.Common.Dtos;
using SwizlyPeasy.Gateway.Services;

namespace SwizlyPeasy.Test.UnitTest.Gateway;

public class ClusterConfigServiceTest
{
    [Fact]
    public async Task GetClusterConfigs_NoServicesAvailable_ReturnsEmptyList()
    {
        var settings = Options.Create(new ServiceDiscoveryConfig());
        var clusterConfigService =
            new ClusterConfigService(TestFactories.GetRetrieveAgentsService(new Dictionary<string, AgentService>()),
                settings);

        var result = await clusterConfigService.RetrieveClustersConfig();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetClusterConfigs_ServicesAvailable_ReturnsListWithServices()
    {
        var discoveryConfig = new ServiceDiscoveryConfig();
        var settings = Options.Create(discoveryConfig);

        var agentsServicesDic = new Dictionary<string, AgentService>
        {
            { "TestService_1", TestFactories.GetAgentService("TestService_1", "TestService") },
            { "SuperService_1", TestFactories.GetAgentService("SuperService_1", "SuperService") },
            { "MegaService_1", TestFactories.GetAgentService("MegaService_1", "MegaService") }
        };

        var clusterConfigService =
            new ClusterConfigService(TestFactories.GetRetrieveAgentsService(agentsServicesDic), settings);
        var result = await clusterConfigService.RetrieveClustersConfig();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        foreach (var clusterConfig in result)
        {
            Assert.NotNull(clusterConfig.Destinations);
            Assert.Single(clusterConfig.Destinations);

            Assert.Equal(discoveryConfig.LoadBalancingPolicy, clusterConfig.LoadBalancingPolicy);

            var destination = clusterConfig.Destinations.First().Value;
            Assert.NotNull(destination);

            var matchingAgentService = agentsServicesDic.Values.First(x => x.Service == clusterConfig.ClusterId);

            Assert.Equal($"{discoveryConfig.Scheme}://{matchingAgentService.Address}:{matchingAgentService.Port}",
                destination.Address);
        }
    }

    [Fact]
    public async Task GetClusterConfigs_WithSameServiceNamesButDifferentIds_ServicesAreGroupedByCluster()
    {
        var discoveryConfig = new ServiceDiscoveryConfig();
        var settings = Options.Create(discoveryConfig);

        var agentsServicesDic = new Dictionary<string, AgentService>
        {
            { "TestService_1", TestFactories.GetAgentService("TestService_1", "TestService") },
            { "SuperService_1", TestFactories.GetAgentService("SuperService_1", "TestService") },
            { "MegaService_1", TestFactories.GetAgentService("MegaService_1", "MegaService") }
        };
        var clusterConfigService =
            new ClusterConfigService(TestFactories.GetRetrieveAgentsService(agentsServicesDic), settings);
        var result = await clusterConfigService.RetrieveClustersConfig();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        foreach (var clusterConfig in result)
        {
            Assert.NotNull(clusterConfig.Destinations);
            Assert.Equal(discoveryConfig.LoadBalancingPolicy, clusterConfig.LoadBalancingPolicy);

            foreach (var destination in clusterConfig.Destinations)
            {
                Assert.NotNull(destination.Value);
                var matchingAgentService = agentsServicesDic.Values.First(x => x.ID == destination.Key);

                Assert.Equal($"{discoveryConfig.Scheme}://{matchingAgentService.Address}:{matchingAgentService.Port}",
                    destination.Value.Address);
            }
        }
    }
}