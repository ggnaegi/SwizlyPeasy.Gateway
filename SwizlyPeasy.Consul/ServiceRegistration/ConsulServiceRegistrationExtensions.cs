using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SwizlyPeasy.Common;
using SwizlyPeasy.Common.Dtos;

namespace SwizlyPeasy.Consul.ServiceRegistration;

/// <summary>
///     Extension method for micro service registration
///     in consul. You should make sure that the RegisterClient extension method
///     is also called.
/// </summary>
public static class ConsulServiceRegistrationExtensions
{
    public static void RegisterService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ServiceRegistrationConfig>(
            configuration.GetSection(Constants.ServiceRegistrationConfigSection));
        services.AddSingleton<IHostedService, ServiceDiscoveryHostedService>();
    }
}