using Consul;
using Microsoft.Extensions.DependencyInjection;
using SwizlyPeasy.Common.Dtos;
using SwizlyPeasy.Consul.KeyValueStore;

namespace SwizlyPeasy.Consul.Agents
{
    /// <summary>
    /// </summary>
    public static class ServiceDiscoveryExtensions
    {
        public static void RegisterConsulClient(this IServiceCollection services, ServiceDiscoveryConfig serviceConfig)
        {
            if (serviceConfig == null)
            {
                throw new ArgumentNullException(nameof(serviceConfig));
            }

            var consulClient = CreateConsulClient(serviceConfig);
            services.AddSingleton<IConsulClient, ConsulClient>(p => consulClient);
            services.AddSingleton<IRetrieveAgentsService, RetrieveAgentsService>();
            services.AddSingleton<IKeyValueService, KeyValueService>();
        }

        private static ConsulClient CreateConsulClient(ServiceDiscoveryConfig serviceConfig)
        {
            return new ConsulClient(config =>
            {
                config.Address = serviceConfig.ServiceDiscoveryAddress;
            });
        }
    }
}