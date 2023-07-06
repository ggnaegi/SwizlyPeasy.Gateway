using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SwizlyPeasy.Common.Dtos;

namespace SwizlyPeasy.Consul.Agents
{
    /// <summary>
    /// </summary>
    public static class ServiceConfigExtensions
    {
        /// <summary>
        ///     Extension method for consul client configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ConfigureConsul(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.Configure<IOptions<ServiceDiscoveryConfig>>(configuration.GetSection(Common.Constants.ServiceDiscoveryConfigSection));

            var serviceDiscoveryConfig = new ServiceDiscoveryConfig();
            configuration.GetSection(Common.Constants.ServiceDiscoveryConfigSection).Bind(serviceDiscoveryConfig);
            services.RegisterConsulClient(serviceDiscoveryConfig);
        }
    }
}