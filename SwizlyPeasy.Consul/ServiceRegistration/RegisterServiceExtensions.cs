using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwizlyPeasy.Common.HealthChecks;
using SwizlyPeasy.Consul.ClientConfig;

namespace SwizlyPeasy.Consul.ServiceRegistration
{
    public static class RegisterServiceExtensions
    {
        public static void RegisterServiceToSwizlyPeasyGateway(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureConsulClient(configuration);
            services.RegisterService(configuration);
            services.AddSwizlyPeasyHealthChecks(configuration);
        }
    }
}
