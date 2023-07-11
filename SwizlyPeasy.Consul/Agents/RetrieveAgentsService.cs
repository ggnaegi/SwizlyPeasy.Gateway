using Consul;

namespace SwizlyPeasy.Consul.Agents;

public class RetrieveAgentsService : IRetrieveAgentsService
{
    private readonly IConsulClient _consulClient;

    public RetrieveAgentsService(IConsulClient consulClient)
    {
        _consulClient = consulClient;
    }

    public async Task<Dictionary<string, IList<AgentService>>> RetrieveAgents()
    {
        var queryResult = await _consulClient.Agent.Services();
        var services = queryResult.Response;

        var servicesDic = new Dictionary<string, IList<AgentService>>();
        if (!services.Any()) return servicesDic;

        foreach (var service in services.Values)
        {
            if (!servicesDic.ContainsKey(service.Service)) servicesDic.Add(service.Service, new List<AgentService>());

            servicesDic[service.Service].Add(service);
        }

        return servicesDic;
    }
}