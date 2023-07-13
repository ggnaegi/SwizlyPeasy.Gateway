using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SwizlyPeasy.Common.Dtos;

namespace SwizlyPeasy.Common.HealthChecks;

/// <summary>
///     Health check extension for
///     micro services configuration
/// </summary>
public static class HealthCheckExtensions
{
    /// <summary>
    ///     health check for services
    ///     not using database connection
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddSwizlyPeasyHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks();
    }

    /// <summary>
    ///     Configuring health endpoint
    /// </summary>
    /// <param name="app"></param>
    public static void UseSwizlyPeasyHealthChecks(this IApplicationBuilder app)
    {
        var config = app.ApplicationServices.GetService<IOptions<ServiceRegistrationConfig>>();
        app.UseHealthChecks($"/{(config == null ? "health": config.Value.HealthCheckPath)}", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var response = new HealthCheckResponse
                {
                    Status = report.Status.ToString(),
                    HealthChecks = report.Entries.Select(x => new HealthCheckResponseItem
                    {
                        Components = x.Key,
                        Status = x.Value.Status.ToString(),
                        Description = x.Value.Description,
                        ExceptionMessage = x.Value.Exception?.Message
                    }),
                    HealthCheckDuration = report.TotalDuration
                };
                await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
            }
        });
    }
}