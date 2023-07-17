using System.Reflection;
using System.Security.Claims;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SwizlyPeasy.Common;
using SwizlyPeasy.Common.Auth;
using SwizlyPeasy.Common.Dtos;
using SwizlyPeasy.Common.Extensions;
using SwizlyPeasy.Common.Middlewares;
using SwizlyPeasy.Consul.ClientConfig;
using SwizlyPeasy.Consul.Health;
using SwizlyPeasy.Gateway.Mediator;
using SwizlyPeasy.Gateway.Mediator.handler;
using SwizlyPeasy.Gateway.Services;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;
using InMemoryConfigProvider = SwizlyPeasy.Gateway.Services.InMemoryConfigProvider;

namespace SwizlyPeasy.Gateway.Extensions;

public static class SetupServices
{
    /// <summary>
    ///     Adding services to the service collection.
    ///     - Authorization Policy OIDC
    ///     - SwizlyPeasy OIDC
    ///     - Consul
    ///     - YARP, with custom configuration provider (retrieving routes from consul KV store, and agents from consul)
    ///     - MediatR
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddSwizlyPeasyGateway(this IServiceCollection services, IConfiguration configuration)
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
        services
            .AddReverseProxy()
            .LoadFromConsul()
            .AddAuthorizationHeaders(configuration)
            .AddStatusService();

        services.AddTransient<IRequestHandler<LoginRequest, UserDto>, LoginHandler>();
        services.AddTransient<IRequestHandler<LogoutRequest, Unit>, LogoutHandler>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }

    /// <summary>
    ///     Setting up pipeline
    /// </summary>
    /// <param name="app"></param>
    public static void UseSwizlyPeasyGateway(this WebApplication app)
    {
        app.UseMiddleware<ExceptionsHandlerMiddleware>();
        app.Use404AsException();
        app.UseRouting();
        app.UseSwizlyPeasyOidc();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.MapReverseProxy();
    }

    /// <summary>
    ///     Adding authorization headers for user authorization
    ///     in micro services
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IReverseProxyBuilder AddAuthorizationHeaders(this IReverseProxyBuilder builder, IConfiguration config)
    {
        builder.AddTransforms(builderContext =>
        {
            // Conditionally add a transform for routes that require auth.
            if (!string.IsNullOrEmpty(builderContext.Route.AuthorizationPolicy))
                builderContext.AddRequestTransform(transformContext =>
                {
                    var claimsConfig = new ClaimsConfig();
                    config.GetSection(Constants.ClaimsConfigSection).Bind(claimsConfig);

                    var userClaims = transformContext.HttpContext.User.Claims.ToArray();

                    // even with options.ClaimActions.MapJsonKey, system can't provide the key sub
                    // that is mapped to ClaimTypes.NameIdentifier
                    foreach (var claimAsHeader in claimsConfig.ClaimsAsHeaders)
                        AddClaimToHeader(userClaims, claimAsHeader, transformContext, claimsConfig.ClaimsHeaderPrefix);

                    return ValueTask.CompletedTask;
                });
        });

        return builder;
    }

    /// <summary>
    /// Adding claim to request's header
    /// reducing methods complexity
    /// </summary>
    /// <param name="userClaims"></param>
    /// <param name="claimAsHeader"></param>
    /// <param name="transformContext"></param>
    /// <param name="claimHeaderPrefix"></param>
    private static void AddClaimToHeader(Claim[] userClaims, string claimAsHeader,
        RequestTransformContext transformContext, string claimHeaderPrefix)
    {
        var foundClaim = userClaims.FirstOrDefault(x => x.Type == claimAsHeader);

        if (claimAsHeader == JwtClaimTypes.Subject)
        {
            foundClaim = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (foundClaim == null) return;

            foundClaim = new Claim(JwtClaimTypes.Subject, foundClaim.Value);
        }

        if (foundClaim == null) return;

        var headerKey = $"{claimHeaderPrefix}-{foundClaim.Type}";
        transformContext.ProxyRequest.Headers.Add(headerKey, foundClaim.Value);
    }

    public static IReverseProxyBuilder AddStatusService(this IReverseProxyBuilder builder)
    {
        builder.Services.AddScoped<IHealthCheckService, HealthCheckService>();
        builder.Services.AddScoped<IStatusService, StatusService>();

        return builder;
    }

    /// <summary>
    ///     Custom configuration load mechanism.
    ///     Inspired by
    ///     https://tanzu.vmware.com/developer/blog/build-api-gateway-csharp-yarp-eureka/
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IReverseProxyBuilder LoadFromConsul(this IReverseProxyBuilder builder)
    {
        builder.Services.AddSingleton<IClusterConfigService, ClusterConfigService>();
        builder.Services.AddSingleton<IRoutesConfigService, RoutesConfigService>();
        builder.Services.AddSingleton<InMemoryConfigProvider>();
        builder.Services.AddSingleton<IHostedService>(ctx => ctx.GetRequiredService<InMemoryConfigProvider>());
        builder.Services.AddSingleton<IProxyConfigProvider>(ctx => ctx.GetRequiredService<InMemoryConfigProvider>());

        return builder;
    }

    /// <summary>
    ///     Adding authorization policy for oidc.
    ///     This policy only require authenticated users.
    ///     The micro services are responsible for the users authorization.
    /// </summary>
    /// <param name="services"></param>
    public static void AddAuthorizationPolicy(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Constants.OidcPolicy, policy => { policy.RequireAuthenticatedUser(); });
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