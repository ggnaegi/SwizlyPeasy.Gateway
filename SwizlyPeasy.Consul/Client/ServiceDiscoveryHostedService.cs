using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SwizlyPeasy.Common.Dtos;

namespace SwizlyPeasy.Consul.Client
{
    /// <summary>
    /// </summary>
    public class ServiceDiscoveryHostedService : IHostedService
    {
        private readonly IConsulClient _client;
        private readonly IOptions<ServiceDiscoveryConfigDto> _config;
        private string _registrationId;

        /// <summary>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="config"></param>
        /// <param name="registrationId"></param>
        public ServiceDiscoveryHostedService(IConsulClient client, IOptions<ServiceDiscoveryConfigDto> config, string registrationId)
        {
            _client = client;
            _config = config;
            _registrationId = registrationId;
        }

        /// <summary>
        ///     Hosted service for consul client
        ///     with health checks (look at zoom.Services.Common.Base.HealthChecks for further information)
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _registrationId = $"{_config.Value.ServiceName}-{_config.Value.ServiceId}";

            var registration = new AgentServiceRegistration {
                ID = _registrationId,
                Name = _config.Value.ServiceName,
                Address = _config.Value.ServiceAddress?.Host,
                Port = _config.Value.ServiceAddress.Port,
                Check = new AgentCheckRegistration {
                    HTTP = $"http://{_config.Value.ServiceAddress?.Host}:{_config.Value.ServiceAddress.Port}/health",
                    Interval = TimeSpan.FromSeconds(10)
                }
            };

            await _client.Agent.ServiceDeregister(registration.ID, cancellationToken);
            await _client.Agent.ServiceRegister(registration, cancellationToken);
        }

        /// <summary>
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.Agent.ServiceDeregister(_registrationId, cancellationToken);
        }
    }
}