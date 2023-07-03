using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SwizlyPeasy.Common.Dtos;

namespace SwizlyPeasy.Consul.Client
{
    /// <summary>
    /// </summary>
    public static class ServiceConfigExtensions
    {
        public const string ServiceDiscoverySection = "ServiceDiscovery";
        /// <summary>
        ///     Extension method for consul client configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="clientOnly"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ConfigureConsul(this IServiceCollection services, IConfiguration configuration, bool clientOnly = false)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.Configure<IOptions<ServiceDiscoveryConfigDto>>(configuration.GetSection(ServiceDiscoverySection));

            var serviceDiscoveryConfig = new ServiceDiscoveryConfigDto();
            configuration.GetSection(ServiceDiscoverySection).Bind(serviceDiscoveryConfig);

            if (clientOnly)
            {
                services.RegisterConsulClient(serviceDiscoveryConfig);
            }
            else
            {
                services.RegisterConsulServices(serviceDiscoveryConfig);
            }
        }
    }
}