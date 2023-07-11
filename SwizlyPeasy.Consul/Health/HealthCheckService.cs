using System.Net;
using Consul;
using SwizlyPeasy.Common.Dtos.Status;

namespace SwizlyPeasy.Consul.Health;

public class HealthCheckService : IHealthCheckService
{
    private readonly IConsulClient _consulClient;

    public HealthCheckService(IConsulClient consulClient)
    {
        _consulClient = consulClient;
    }

    public async Task<bool> IsServiceHealthy(string serviceId)
    {
        var serviceHealth =
            await _consulClient.Raw.Query($"/v1/agent/health/service/id/{serviceId}", new QueryOptions());

        if (serviceHealth.StatusCode != HttpStatusCode.OK) return false;

        if (serviceHealth.Response == null) return false;

        HealthEndpointStatusDto status = serviceHealth.Response.ToObject(typeof(HealthEndpointStatusDto));

        return status.AggregatedStatus != null && HealthStatus.Passing.Status == status.AggregatedStatus;
    }
}