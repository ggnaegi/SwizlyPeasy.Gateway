using System.Runtime.CompilerServices;
using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SwizlyPeasy.Common.Dtos;

namespace SwizlyPeasy.Consul.Client
{
    /// <summary>
    /// </summary>
    public static class ServiceDiscoveryExtensions
    {
        /// <summary>
        ///     Registering consul services (mainly consul client)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceConfig"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void RegisterConsulServices(this IServiceCollection services, ServiceDiscoveryConfigDto serviceConfig)
        {
            if (serviceConfig == null)
            {
                throw new ArgumentNullException(nameof(serviceConfig));
            }

            var consulClient = CreateConsulClient(serviceConfig);

            services.AddSingleton(serviceConfig);
            services.AddSingleton<IHostedService, ServiceDiscoveryHostedService>();
            services.AddSingleton<IConsulClient, ConsulClient>(p => consulClient);
            services.AddSingleton<IRetrieveHostService, RetrieveHostService>();
        }

        public static void RegisterConsulClient(this IServiceCollection services, ServiceDiscoveryConfigDto serviceConfig)
        {
            if (serviceConfig == null)
            {
                throw new ArgumentNullException(nameof(serviceConfig));
            }

            var consulClient = CreateConsulClient(serviceConfig);
            services.AddSingleton<IConsulClient, ConsulClient>(p => consulClient);
            services.AddSingleton<IRetrieveHostService, RetrieveHostService>();
        }

        private static ConsulClient CreateConsulClient(ServiceDiscoveryConfigDto serviceConfig)
        {
            return new ConsulClient(config =>
            {
                config.Address = serviceConfig.ServiceDiscoveryAddress;
            });
        }
    }
}