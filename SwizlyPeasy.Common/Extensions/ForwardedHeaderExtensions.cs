using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SwizlyPeasy.Common.Dtos;
using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Common.Extensions;

public static class ForwardedHeaderExtensions
{
    /// <summary>
    ///     Adding known proxies if Gateway behind one or more
    ///     proxies
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddKnownProxiesAndNetworks(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ProxiesAndNetworksConfig>(configuration.GetSection(Constants.ProxiesAndNetworksConfigSection));

        var proxiesAndNetworksConfig = new ProxiesAndNetworksConfig();
        configuration.GetSection(Constants.ProxiesAndNetworksConfigSection).Bind(proxiesAndNetworksConfig);
        if (!proxiesAndNetworksConfig.UseForwardedHeaders) return;

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.ForwardLimit = proxiesAndNetworksConfig.ForwardLimit;

            // Only loopback proxies are allowed by default.
            // Clear that restriction because forwarders are enabled by explicit 
            // configuration.
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();

            foreach (var proxy in proxiesAndNetworksConfig.Proxies) options.KnownProxies.Add(IPAddress.Parse(proxy));

            // assuming prefix length = 24
            foreach (var network in proxiesAndNetworksConfig.KnownNetworks) options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse(network), 24));
        });
    }

    public static void UseKnownProxiesAndNetworks(this IApplicationBuilder app)
    {
        var proxiesAndNetworkConfig = app.ApplicationServices.GetService<IOptions<ProxiesAndNetworksConfig>>();
        var logger = app.ApplicationServices.GetRequiredService<ILogger<ProxiesAndNetworksConfig>>();

        if (proxiesAndNetworkConfig == null)
        {
            const string errorMessage = "proxies and networks config can't be found, please check the app settings!";
            logger.LogCritical(errorMessage);
            throw new InternalDomainException(errorMessage, null);
        }

        if (!proxiesAndNetworkConfig.Value.UseForwardedHeaders)
        {
            return;
        }

        app.UseForwardedHeaders();
    }
}