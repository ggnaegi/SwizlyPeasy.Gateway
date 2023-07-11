using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwizlyPeasy.Common;
using SwizlyPeasy.Common.Dtos;
using SwizlyPeasy.Consul.Agents;
using SwizlyPeasy.Consul.KeyValueStore;

namespace SwizlyPeasy.Consul.ClientConfig;

/// <summary>
/// </summary>
public static class ConsulClientConfigurationExtensions
{
    /// <summary>
    ///     Extension method for consul client configuration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void ConfigureConsulClient(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        services.Configure<ServiceDiscoveryConfig>(configuration.GetSection(Constants.ServiceDiscoveryConfigSection));
        var serviceDiscoveryConfig = new ServiceDiscoveryConfig();
        configuration.GetSection(Constants.ServiceDiscoveryConfigSection).Bind(serviceDiscoveryConfig);

        var consulClient = CreateConsulClient(serviceDiscoveryConfig);
        services.AddSingleton<IConsulClient, ConsulClient>(p => consulClient);
        services.AddSingleton<IRetrieveAgentsService, RetrieveAgentsService>();
        services.AddSingleton<IKeyValueService, KeyValueService>();
    }

    /// <summary>
    ///     Creating consul client instance.
    /// </summary>
    /// <param name="serviceConfig"></param>
    /// <returns></returns>
    private static ConsulClient CreateConsulClient(ServiceDiscoveryConfig serviceConfig)
    {
        return new ConsulClient(config => { config.Address = serviceConfig.ServiceDiscoveryAddress; });
    }
}