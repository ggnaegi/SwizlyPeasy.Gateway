using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwizlyPeasy.Common.Dtos;

namespace SwizlyPeasy.Common.Auth;

public static class ClientAuthExtensions
{
    /// <summary>
    ///     Setting "dummy" authentication
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    public static void SetSwizlyPeasyAuthentication(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<ClaimsConfig>(config.GetSection(Constants.ClaimsConfigSection));
        services.AddAuthentication(SwizlyPeasyAuthenticationOptions.DefaultScheme)
            .AddScheme<SwizlyPeasyAuthenticationOptions, SwizlyPeasyClientAuthenticationHandler>
            (SwizlyPeasyAuthenticationOptions.DefaultScheme,
                _ => { });
    }
}