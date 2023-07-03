using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using SwizlyPeasy.Common;
using SwizlyPeasy.Common.Auth;
using SwizlyPeasy.Common.Dtos;
using SwizlyPeasy.Common.Middlewares;
using SwizlyPeasy.Gateway.Mediator;
using SwizlyPeasy.Gateway.Mediator.handler;

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
        services.AddSwizlyPeasyOpenIdConnect(configuration);
        services.AddReverseProxy().LoadFromConfig(configuration.GetSection(Constants.ReverseProxyConfigSection));
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
        app.MapControllers();
        app.MapReverseProxy();
        app.UseMiddleware<ExceptionsHandlerMiddleware>();
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