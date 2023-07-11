namespace SwizlyPeasy.Consul.Health;

public interface IHealthCheckService
{
    public Task<bool> IsServiceHealthy(string serviceId);
}