using NLog;
using NLog.Web;
using SwizlyPeasy.Gateway.Extensions;

var logger = LogManager.Setup().LoadConfigurationFromFile("NLog.config").GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSwizlyPeasyGateway(builder.Configuration);

    builder.Host.UseNLog();

    var app = builder.Build();

    app.UseSwizlyPeasyGateway();
    app.Run();
}
catch (Exception e)
{
    logger.Error(e, "Stopped program because of exception");
}
finally
{
    LogManager.Shutdown();
}

namespace SwizlyPeasy.Gateway.API
{
    /// <summary>
    ///     For Integration Tests...
    /// </summary>
    public class Program
    {
    }
}