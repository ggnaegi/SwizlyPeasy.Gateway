using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwizlyPeasy.Common.Dtos;
using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Common.Extensions;

public static class RateLimiterExtensions
{
    /// <summary>
    /// Retrieving the client IP Address
    /// The result is null if the connection isn't a TCP connection, e.g., a Unix Domain Socket or a transport that isn't TCP based.
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static string ResolveClientIpAddress(this HttpContext httpContext)
    {
        return httpContext.Connection.RemoteIpAddress == null ? "unknown" : httpContext.Connection.RemoteIpAddress.ToString();
    }

    /// <summary>
    /// Adding custom rate limiters
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <exception cref="TooManyRequestsException"></exception>
    public static void AddSwizlyPeasyRateLimiters(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRateLimiter(options =>
        {
            options.OnRejected = (context, _) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    throw new TooManyRequestsException(
                        $"Too many requests. Please try again after {retryAfter.TotalMinutes} minute(s). ", null);

                throw new TooManyRequestsException("Too many requests. Please try again later.", null);
            };

            var rateLimiterPolicyConfigs = new List<RateLimiterPolicyConfig>();
            configuration.GetSection(Constants.RateLimiterPoliciesSection).Bind(rateLimiterPolicyConfigs);

            foreach (var policyConfig in rateLimiterPolicyConfigs) options.AddSwizlyPeasyPolicy(policyConfig);
        });
    }

    /// <summary>
    /// Using client IP address as partition key
    /// </summary>
    /// <param name="options"></param>
    /// <param name="config"></param>
    /// <exception cref="InternalDomainException"></exception>
    private static void AddSwizlyPeasyPolicy(this RateLimiterOptions options, RateLimiterPolicyConfig config)
    {
        switch (config.RateLimiterType)
        {
            case Constants.ChainedRateLimiter:
                throw new NotImplementedException("Chained rate limiters not yet implemented...");
            case nameof(FixedWindowRateLimiter):
                options.AddPolicy(config.PolicyName, context =>
                    {
                        return RateLimitPartition.GetFixedWindowLimiter(context.ResolveClientIpAddress(), _ =>
                            new FixedWindowRateLimiterOptions
                            {
                                AutoReplenishment = config.AutoReplenishment,
                                PermitLimit = config.PermitLimit,
                                QueueLimit = config.QueueLimit,
                                QueueProcessingOrder = (QueueProcessingOrder)config.QueueProcessingOrder,
                                Window = TimeSpan.FromSeconds(config.Window)
                            });
                    }
                );
                return;
            case nameof(SlidingWindowRateLimiter):
                options.AddPolicy(config.PolicyName, context =>
                    {
                        return RateLimitPartition.GetSlidingWindowLimiter(context.ResolveClientIpAddress(), _ =>
                            new SlidingWindowRateLimiterOptions
                            {
                                AutoReplenishment = config.AutoReplenishment,
                                PermitLimit = config.PermitLimit,
                                QueueProcessingOrder = (QueueProcessingOrder)config.QueueProcessingOrder,
                                QueueLimit = config.QueueLimit,
                                Window = TimeSpan.FromSeconds(config.Window),
                                SegmentsPerWindow = config.SegmentsPerWindow
                            });
                    }
                );
                return;
            case nameof(ConcurrencyLimiter):
                options.AddPolicy(config.PolicyName, context =>
                    {
                        return RateLimitPartition.GetConcurrencyLimiter(context.ResolveClientIpAddress(), _ =>
                            new ConcurrencyLimiterOptions
                            {
                                PermitLimit = config.PermitLimit,
                                QueueLimit = config.QueueLimit,
                                QueueProcessingOrder = (QueueProcessingOrder)config.QueueProcessingOrder
                            });
                    }
                );
                return;
            case nameof(TokenBucketRateLimiter):
                options.AddPolicy(config.PolicyName, context =>
                    {
                        return RateLimitPartition.GetTokenBucketLimiter(context.ResolveClientIpAddress(), _ =>
                            new TokenBucketRateLimiterOptions
                            {
                                AutoReplenishment = config.AutoReplenishment,
                                QueueLimit = config.QueueLimit,
                                QueueProcessingOrder = (QueueProcessingOrder)config.QueueProcessingOrder,
                                ReplenishmentPeriod = TimeSpan.FromSeconds(config.ReplenishmentPeriod),
                                TokenLimit = config.TokenLimit,
                                TokensPerPeriod = config.TokensPerPeriod
                            });
                    }
                );
                return;
            default:
                throw new InternalDomainException(
                    $"The rate limiter type {config.RateLimiterType} is unknown, please check the app settings.",
                    null);
        }
    }
}