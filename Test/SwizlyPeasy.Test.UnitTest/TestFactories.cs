using System.Text;
using Consul;
using SwizlyPeasy.Consul.Agents;
using SwizlyPeasy.Consul.Health;
using SwizlyPeasy.Consul.KeyValueStore;

namespace SwizlyPeasy.Test.UnitTest;

public static class TestFactories
{
    public const string RouteConfigKey = "SwizlyPeasy.Key";

    public const string RouteConfigString =
        "{\"Routes\":{\"route1\":{\"ClusterId\":\"DemoAPI\",\"AuthorizationPolicy\":\"oidc\",\"Match\":{\"Path\":\"/api/v1/demo/weather\"},\"Transforms\":[{\"RequestHeader\":\"Accept-Language\",\"Set\":\"de-CH\"}]},\"route2\":{\"ClusterId\":\"DemoAPI\",\"AuthorizationPolicy\":\"oidc\",\"Match\":{\"Path\":\"/api/v1/demo/weather-with-authorization\"},\"Transforms\":[{\"RequestHeader\":\"Accept-Language\",\"Set\":\"de-CH\"}]}}}";

    public const string ServiceUrl = "/v1/agent/health/service/id/DemoAPI-1";
    public const string ServiceId = "DemoAPI-1";

    public const string WrongUrl = "/v1/agent/health/service/id/WrongAPI-1";
    public const string WrongId = "WrongAPI-1";

    public const string EmptyUrl = "/v1/agent/health/service/id/EmptyAPI-1";
    public const string EmptyId = "EmptyAPI-1";

    public const string HealthCheckOkResult =
        "{\"AggregatedStatus\":\"passing\",\"Service\":{\"ID\":\"DemoAPI-1\",\"Service\":\"DemoAPI\",\"Tags\":[],\"Meta\":{},\"Port\":8020,\"Address\":\"localhost\",\"Weights\":{\"Passing\":1,\"Warning\":1},\"EnableTagOverride\":false,\"Datacenter\":\"dc1\"},\"Checks\":[{\"Node\":\"ANB194\",\"CheckID\":\"service:DemoAPI-1\",\"Name\":\"Service'DemoAPI'check\",\"Status\":\"passing\",\"Notes\":\"\",\"Output\":\"HTTPGEThttp://localhost:8020/health:200OKOutput:{\\\"Status\\\":\\\"Healthy\\\",\\\"HealthChecks\\\":[],\\\"HealthCheckDuration\\\":\\\"00:00:00.0000114\\\"}\",\"ServiceID\":\"DemoAPI-1\",\"ServiceName\":\"DemoAPI\",\"ServiceTags\":null,\"Type\":\"\",\"ExposedPort\":0,\"Definition\":{\"Interval\":\"0s\",\"Timeout\":\"0s\",\"DeregisterCriticalServiceAfter\":\"0s\",\"HTTP\":\"\",\"Header\":null,\"Method\":\"\",\"Body\":\"\",\"TLSServerName\":\"\",\"TLSSkipVerify\":false,\"TCP\":\"\",\"UDP\":\"\",\"GRPC\":\"\",\"OSService\":\"\",\"GRPCUseTLS\":false},\"CreateIndex\":0,\"ModifyIndex\":0}]}";

    public const string HealthCheckNotFoundResult = "{\"AggregatedStatus\":\"critical\",\"Service\":null,\"Checks\":null}";


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

    public static HealthCheckService GetHealthCheckService()
    {
        var consulClient = ConsulClientFactory.GetRawConsulClient();
        return new HealthCheckService(consulClient);
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