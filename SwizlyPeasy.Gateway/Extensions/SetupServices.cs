using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using SwizlyPeasy.Common.Auth;
using SwizlyPeasy.Common.Dtos;
using SwizlyPeasy.Common.Middlewares;
using SwizlyPeasy.Consul.Agents;
using SwizlyPeasy.Gateway.Mediator;
using SwizlyPeasy.Gateway.Mediator.handler;
using SwizlyPeasy.Gateway.Services;
using Yarp.ReverseProxy.Configuration;
using InMemoryConfigProvider = SwizlyPeasy.Gateway.Services.InMemoryConfigProvider;

namespace SwizlyPeasy.Gateway.Extensions;

public static class SetupServices
{
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
        services.ConfigureConsul(configuration);
        services.AddReverseProxy().LoadFromConsul();
        services.AddTransient<IRequestHandler<LoginRequest, UserDto>, LoginHandler>();
        services.AddTransient<IRequestHandler<LogoutRequest, Unit>, LogoutHandler>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }

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

    public static void AddAuthorizationPolicy(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("oidc", policy => policy.RequireAuthenticatedUser());
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