namespace SwizlyPeasy.Consul.Health;

public interface IHealthCheckService
{
    /// <summary>
    /// Checking if service's destination is healthy
    /// (checks are passing)
    /// </summary>
    /// <param name="serviceId"></param>
    /// <returns></returns>
    public Task<bool> IsServiceHealthy(string serviceId);
}