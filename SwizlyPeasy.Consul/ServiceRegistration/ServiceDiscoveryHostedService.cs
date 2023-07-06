using Consul;
using Microsoft.Extensions.Hosting;
using SwizlyPeasy.Common.Dtos;

namespace SwizlyPeasy.Consul.ServiceRegistration;

public class ServiceDiscoveryHostedService : IHostedService
{
    private readonly IConsulClient _client;
    private readonly ServiceRegistrationConfig _config;
    private string _registrationId = "";

    /// <summary>
    /// </summary>
    /// <param name="client"></param>
    /// <param name="config"></param>
    public ServiceDiscoveryHostedService(IConsulClient client, ServiceRegistrationConfig config)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    ///     registering service to consul.
    ///     The registration id is defined by "service name" and "service id".
    ///     This allows load balancing, with services grouped by service name.
    ///     The service id must therefore be unique for a given service name.
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _registrationId = $"{_config.ServiceName}-{_config.ServiceId}";

        var registration = new AgentServiceRegistration
        {
            ID = _registrationId,
            Name = _config.ServiceName,
            Address = _config.ServiceAddress.Host,
            Port = _config.ServiceAddress.Port,
            Check = new AgentCheckRegistration
            {
                HTTP =
                    $"{_config.ServiceAddress.Scheme}://{_config.ServiceAddress.Host}:{_config.ServiceAddress.Port}/{_config.HealthCheckPath}",
                Interval = TimeSpan.FromSeconds(10)
            }
        };

        await _client.Agent.ServiceDeregister(registration.ID, cancellationToken);
        await _client.Agent.ServiceRegister(registration, cancellationToken);
    }

    /// <summary>
    ///     unregistering the service from consul
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.Agent.ServiceDeregister(_registrationId, cancellationToken);
    }
}