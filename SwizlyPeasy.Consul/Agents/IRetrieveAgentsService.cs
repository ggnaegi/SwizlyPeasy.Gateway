using Consul;

namespace SwizlyPeasy.Consul.Agents
{
    public interface IRetrieveAgentsService
    {
        Task<Dictionary<string, IList<AgentService>>> RetrieveAgents();
    }
}