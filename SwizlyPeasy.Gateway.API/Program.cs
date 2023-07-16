using NLog;
using SwizlyPeasy.Gateway.Extensions;

var logger = LogManager.Setup().LoadConfigurationFromFile("NLog.config").GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSwizlyPeasyGateway(builder.Configuration);

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

/// <summary>
/// For Integration Tests...
/// </summary>
public partial class Program
{
}