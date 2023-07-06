namespace SwizlyPeasy.Common.Dtos;

/// <summary>
/// Consul service registration
/// </summary>
public class ServiceRegistrationConfig
{
    public string ServiceName { get; set; } = "SwizlyPeasy.Service";
    public string ServiceId { get; set; } = "1";
    public Uri ServiceAddress { get; set; } = new("http://localhost:8010");
    public string HealthCheckPath { get; set; } = "health";
}