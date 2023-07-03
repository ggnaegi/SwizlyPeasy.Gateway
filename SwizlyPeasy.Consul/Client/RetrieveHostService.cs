using Consul;
using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Consul.Client
{
    /// <summary>
    ///     Service used to retrieve host ip/dns name and port
    ///     using consul, with micro service name as parameter
    /// </summary>
    public class RetrieveHostService : IRetrieveHostService
    {
        private readonly IConsulClient _consulClient;

        public RetrieveHostService(IConsulClient consulClient)
        {
            _consulClient = consulClient;
        }

        public async Task<Uri> RetrieveHostFromServiceName(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new InternalDomainException("Service name can't null or empty");
            }

            var queryResult = await _consulClient.Agent.Services();
            var services = queryResult.Response;

            if (!services.Any())
            {
                throw new InternalDomainException("No services configured!");
            }

            var matchingServiceAgents = services.Values
                .Where(x => x.Service == serviceName)
                .ToList();


            if (!matchingServiceAgents.Any())
            {
                throw new InternalDomainException($"Service with name {serviceName} couldn't be found!");
            }

            var random = new Random();

            //randomly choosing an available agent
            //should maybe use the whole list with polly
            var agent = matchingServiceAgents[random.Next(0, matchingServiceAgents.Count)];
            return new Uri($"{agent.Address}:{agent.Port}");
        }
    }
}