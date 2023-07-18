using System.Net;
using Consul;
using SwizlyPeasy.Common.Dtos.Status;
using SwizlyPeasy.Common.Exceptions;

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
        QueryResult<dynamic>? serviceHealth;

        // handling exceptions from consul client
        // consul could throw a ConsulRequestException with status 503
        try
        {
            serviceHealth = await _consulClient.Raw.Query($"/v1/agent/health/service/id/{serviceId}", new QueryOptions());
        }
        catch (ConsulRequestException)
        {
            return false;
        }
        

        if (serviceHealth.StatusCode == HttpStatusCode.NotFound)
            throw new InternalDomainException($"No Health Checks for service with Id {serviceId} can be found.", null);

        if (serviceHealth.Response == null)
            throw new InternalDomainException($"Consul returned an empty response for service with Id {serviceId}",
                null);

        HealthEndpointStatusDto status = serviceHealth.Response.ToObject(typeof(HealthEndpointStatusDto));

        return status.AggregatedStatus != null && HealthStatus.Passing.Status == status.AggregatedStatus;
    }
}