using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using SwizlyPeasy.Common.Auth;
using SwizlyPeasy.Common.Dtos;
using SwizlyPeasy.Common.Middlewares;
using SwizlyPeasy.Consul.ClientConfig;
using SwizlyPeasy.Gateway.Mediator;
using SwizlyPeasy.Gateway.Mediator.handler;
using SwizlyPeasy.Gateway.Services;
using Yarp.ReverseProxy.Configuration;
using InMemoryConfigProvider = SwizlyPeasy.Gateway.Services.InMemoryConfigProvider;

namespace SwizlyPeasy.Gateway.Extensions;

public static class SetupServices
{
    /// <summary>
    /// Adding services to the service collection.
    /// - Authorization Policy OIDC
    /// - SwizlyPeasy OIDC
    /// - Consul
    /// - YARP, with custom configuration provider (retrieving routes from consul KV store, and agents from consul)
    /// - MediatR
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void SetupGatewayServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddHttpClient();
        services.AddControllers(options =>
        {
            options.ReturnHttpNotAcceptable = true;
            options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
        }).AddNewtonsoftJson(opt => { opt.UseMemberCasing(); });
        services.AddAuthorizationPolicy();
        services.AddSwizlyPeasyOpenIdConnect(configuration);
        services.ConfigureConsulClient(configuration);
        services.AddReverseProxy().LoadFromConsul();
        services.AddTransient<IRequestHandler<LoginRequest, UserDto>, LoginHandler>();
        services.AddTransient<IRequestHandler<LogoutRequest, Unit>, LogoutHandler>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }

    /// <summary>
    /// Setting up pipeline
    /// </summary>
    /// <param name="app"></param>
    public static void SetupMiddleWares(this WebApplication app)
    {
        app.UseRouting();
        app.UseSwizlyPeasyOidc();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<ExceptionsHandlerMiddleware>();
        app.MapControllers();
        app.MapReverseProxy();
    }

    /// <summary>
    /// Custom configuration load mechanism.
    /// Inspired by
    /// https://tanzu.vmware.com/developer/blog/build-api-gateway-csharp-yarp-eureka/
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IReverseProxyBuilder LoadFromConsul(this IReverseProxyBuilder builder)
    {
        builder.Services.AddSingleton<IClusterConfigService, ClusterConfigService>();
        builder.Services.AddSingleton<IRoutesConfigService, RoutesConfigService>();

        builder.Services.AddSingleton(sp =>
            new InMemoryConfigProvider(
                sp.GetRequiredService<IClusterConfigService>(),
                sp.GetRequiredService<IRoutesConfigService>(),
                sp.GetRequiredService<IOptions<ServiceDiscoveryConfig>>()));

        builder.Services.AddSingleton<IHostedService>(ctx => ctx.GetRequiredService<InMemoryConfigProvider>());
        builder.Services.AddSingleton<IProxyConfigProvider>(ctx => ctx.GetRequiredService<InMemoryConfigProvider>());

        return builder;
    }

    /// <summary>
    /// Adding authorization policy for oidc.
    /// This policy only require authenticated users.
    /// The micro services are responsible for the users authorization.
    /// </summary>
    /// <param name="services"></param>
    public static void AddAuthorizationPolicy(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Common.Constants.OidcPolicy, policy => policy.RequireAuthenticatedUser());
        });
    }

    /// <summary>
    ///     Json patch support, without disabling default json support.
    /// </summary>
    /// <returns></returns>
    private static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
    {
        var builder = new ServiceCollection()
            .AddLogging()
            .AddMvc()
            .AddNewtonsoftJson()
            .Services.BuildServiceProvider();

        return builder
            .GetRequiredService<IOptions<MvcOptions>>()
            .Value
            .InputFormatters
            .OfType<NewtonsoftJsonPatchInputFormatter>()
            .First();
    }
}