using Consul;

namespace SwizlyPeasy.Consul.Agents
{
    public interface IRetrieveAgentsService
    {
        /// <summary>
        /// Retrieving registered services from consul
        /// The services are grouped by service name.
        /// This makes it easier to create the cluster part of the yarp configuration
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, IList<AgentService>>> RetrieveAgents();
    }
}