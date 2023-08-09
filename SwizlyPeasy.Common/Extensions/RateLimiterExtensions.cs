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
    public static void AddSwizlyPeasyRateLimiters(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = 429;
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

    private static void AddSwizlyPeasyPolicy(this RateLimiterOptions options, RateLimiterPolicyConfig config)
    {
        switch (config.RateLimiterType)
        {
            case nameof(FixedWindowRateLimiter):
                options.AddFixedWindowLimiter(config.PolicyName, opt =>
                {
                    opt.AutoReplenishment = config.AutoReplenishment;
                    opt.PermitLimit = config.PermitLimit;
                    opt.QueueLimit = config.QueueLimit;
                    opt.QueueProcessingOrder = (QueueProcessingOrder)config.QueueProcessingOrder;
                    opt.Window = TimeSpan.FromSeconds(config.Window);
                });
                return;
            case nameof(SlidingWindowRateLimiter):
                options.AddSlidingWindowLimiter(config.PolicyName, opt =>
                {
                    opt.AutoReplenishment = config.AutoReplenishment;
                    opt.PermitLimit = config.PermitLimit;
                    opt.QueueProcessingOrder = (QueueProcessingOrder)config.QueueProcessingOrder;
                    opt.QueueLimit = config.QueueLimit;
                    opt.Window = TimeSpan.FromSeconds(config.Window);
                    opt.SegmentsPerWindow = config.SegmentsPerWindow;
                });
                return;
            case nameof(ConcurrencyLimiterOptions):
                options.AddConcurrencyLimiter(config.PolicyName, opt =>
                {
                    opt.PermitLimit = config.PermitLimit;
                    opt.QueueLimit = config.QueueLimit;
                    opt.QueueProcessingOrder = (QueueProcessingOrder)config.QueueProcessingOrder;
                });
                return;
            case nameof(TokenBucketRateLimiter):
                options.AddTokenBucketLimiter(config.PolicyName, opt =>
                {
                    opt.AutoReplenishment = config.AutoReplenishment;
                    opt.QueueLimit = config.QueueLimit;
                    opt.QueueProcessingOrder = (QueueProcessingOrder)config.QueueProcessingOrder;
                    opt.ReplenishmentPeriod = TimeSpan.FromSeconds(config.ReplenishmentPeriod);
                    opt.TokenLimit = config.TokenLimit;
                    opt.TokensPerPeriod = config.TokensPerPeriod;
                });
                return;
            default:
                throw new InternalDomainException(
                    $"The rate limiter type {config.RateLimiterType} is unknown, please check the app settings.",
                    null);
        }
    }
}