using Consul;
using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Consul.Agents
{
    /// <summary>
    ///     Service used to retrieve host ip/dns name and port
    ///     using consul, with micro service name as parameter
    /// </summary>
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

            if (!services.Any())
            {
                throw new InternalDomainException("No services configured!");
            }

            var servicesDic = new Dictionary<string, IList<AgentService>>();
            foreach (var service in services.Values)
            {
                if (!servicesDic.ContainsKey(service.Service))
                {
                    servicesDic.Add(service.Service, new List<AgentService>());
                }

                servicesDic[service.Service].Add(service);
            }
            return servicesDic;
        }
    }
}