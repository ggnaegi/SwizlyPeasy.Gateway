using System.Text;
using Consul;
using SwizlyPeasy.Consul.Agents;
using SwizlyPeasy.Consul.KeyValueStore;

namespace SwizlyPeasy.Test.UnitTest;

public static class TestFactories
{
    public const string RouteConfigKey = "SwizlyPeasy.Key";

    public const string RouteConfigString =
        "{\"Routes\":{\"route1\":{\"ClusterId\":\"DemoAPI\",\"AuthorizationPolicy\":\"oidc\",\"Match\":{\"Path\":\"/api/v1/demo/weather\"},\"Transforms\":[{\"RequestHeader\":\"Accept-Language\",\"Set\":\"de-CH\"}]},\"route2\":{\"ClusterId\":\"DemoAPI\",\"AuthorizationPolicy\":\"oidc\",\"Match\":{\"Path\":\"/api/v1/demo/weather-with-authorization\"},\"Transforms\":[{\"RequestHeader\":\"Accept-Language\",\"Set\":\"de-CH\"}]}}}";

    public static KeyValueService GetKeyValueService()
    {
        var consulClient = ConsulClientFactory.GetKvConsulClient();
        return new KeyValueService(consulClient);
    }

    public static RetrieveAgentsService GetRetrieveAgentsService(Dictionary<string, AgentService> servicesDic)
    {
        var consulClient = ConsulClientFactory.GetAgentsConsulClient(servicesDic);
        return new RetrieveAgentsService(consulClient);
    }

    public static KVPair GetKvPair()
    {
        var routeConfigBytes = Encoding.UTF8.GetBytes(RouteConfigString);
        return new KVPair(RouteConfigKey)
        {
            Value = routeConfigBytes
        };
    }

    public static AgentService GetAgentService(string instanceId, string serviceName)
    {
        return new AgentService
        {
            Service = serviceName,
            ID = instanceId,
            Address = "localhost",
            Port = 8020
        };
    }
}