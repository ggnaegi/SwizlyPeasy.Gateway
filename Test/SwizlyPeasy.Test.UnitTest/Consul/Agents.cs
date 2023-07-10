using Consul;

namespace SwizlyPeasy.Test.UnitTest.Consul;

public class Agents
{
    [Fact]
    public async Task GetServices_NoRegistered_ReturnsEmptyDictionary()
    {
        var retrieveAgentsService = TestFactories.GetRetrieveAgentsService(new Dictionary<string, AgentService>());
        var result = await retrieveAgentsService.RetrieveAgents();
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetServices_SomeRegistered_ReturnsRegisteredInDictionary()
    {
        var agentsServicesDic = new Dictionary<string, AgentService>
        {
            { "TestService_1", TestFactories.GetAgentService("TestService_1", "TestService") },
            { "SuperService_1", TestFactories.GetAgentService("SuperService_1", "SuperService") },
            { "MegaService_1", TestFactories.GetAgentService("MegaService_1", "MegaService") }
        };
        var retrieveAgentsService = TestFactories.GetRetrieveAgentsService(agentsServicesDic);

        var foundServices = await retrieveAgentsService.RetrieveAgents();

        Assert.Equal(3, foundServices.Keys.Count);

        Assert.Contains("TestService", foundServices.Keys);
        Assert.Contains("SuperService", foundServices.Keys);
        Assert.Contains("MegaService", foundServices.Keys);

        Assert.Single(foundServices["TestService"]);
        Assert.Single(foundServices["SuperService"]);
        Assert.Single(foundServices["MegaService"]);

        Assert.Equal("TestService_1", foundServices["TestService"].First().ID);
        Assert.Equal("SuperService_1", foundServices["SuperService"].First().ID);
        Assert.Equal("MegaService_1", foundServices["MegaService"].First().ID);
    }

    [Fact]
    public async Task GetServices_WithSameServiceNamesButDifferentIds_ReturnsRegisteredGroupedByServiceNameDictionary()
    {
        var agentsServicesDic = new Dictionary<string, AgentService>
        {
            { "TestService_1", TestFactories.GetAgentService("TestService_1", "TestService") },
            { "SuperService_1", TestFactories.GetAgentService("SuperService_1", "TestService") },
            { "MegaService_1", TestFactories.GetAgentService("MegaService_1", "MegaService") }
        };
        var retrieveAgentsService = TestFactories.GetRetrieveAgentsService(agentsServicesDic);

        var foundServices = await retrieveAgentsService.RetrieveAgents();

        Assert.Equal(2, foundServices.Keys.Count);

        Assert.Contains("TestService", foundServices.Keys);
        Assert.DoesNotContain("SuperService", foundServices.Keys);
        Assert.Contains("MegaService", foundServices.Keys);

        Assert.Equal(2, foundServices["TestService"].Count);
        Assert.Single(foundServices["MegaService"]);

        Assert.Equal("TestService_1", foundServices["TestService"].First(x => x.ID == "TestService_1").ID);
        Assert.Equal("SuperService_1", foundServices["TestService"].First(x => x.ID == "SuperService_1").ID);
        Assert.Equal("MegaService_1", foundServices["MegaService"].First().ID);
    }
}