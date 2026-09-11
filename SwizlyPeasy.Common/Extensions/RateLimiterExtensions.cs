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
    ///     Retrieving the client IP Address
    ///     The result is null if the connection isn't a TCP connection, e.g., a Unix Domain Socket or a transport that isn't
    ///     TCP based.
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static string ResolveClientIpAddress(this HttpContext httpContext)
    {
        return httpContext.Connection.RemoteIpAddress == null
            ? "unknown"
            : httpContext.Connection.RemoteIpAddress.ToString();
    }

    /// <summary>
    ///     Adding custom rate limiters
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

            var chainedRateLimiterPolicyConfigs = new List<ChainedRateLimiterPolicyConfig>();
            configuration.GetSection(Constants.ChainedRateLimiterPoliciesSection).Bind(chainedRateLimiterPolicyConfigs);

            ValidatePolicyNames(rateLimiterPolicyConfigs.Cast<IRateLimiterPolicyConfig>()
                .Concat(chainedRateLimiterPolicyConfigs));
            foreach (var policyConfig in rateLimiterPolicyConfigs) options.AddSwizlyPeasyPolicy(policyConfig);

            var policiesByName = rateLimiterPolicyConfigs.ToDictionary(policy => policy.PolicyName,
                StringComparer.Ordinal);
            foreach (var chainedPolicyConfig in chainedRateLimiterPolicyConfigs)
                options.AddSwizlyPeasyChainedPolicy(chainedPolicyConfig, policiesByName);
        });
    }

    /// <summary>
    ///     Using client IP address as partition key
    /// </summary>
    /// <param name="options"></param>
    /// <param name="config"></param>
    /// <exception cref="InternalDomainException"></exception>
    private static void AddSwizlyPeasyPolicy(this RateLimiterOptions options, RateLimiterPolicyConfig config)
    {
        ValidatePolicyName(config.PolicyName);
        ValidateRateLimiterConfig(config, config.PolicyName);
        options.AddPolicy(config.PolicyName, context =>
        {
            return RateLimitPartition.Get(context.ResolveClientIpAddress(), _ => CreateRateLimiter(config));
        });
    }

    private static void AddSwizlyPeasyChainedPolicy(this RateLimiterOptions options,
        ChainedRateLimiterPolicyConfig config, IReadOnlyDictionary<string, RateLimiterPolicyConfig> policiesByName)
    {
        ValidatePolicyName(config.PolicyName);
        if (config.RateLimiterPolicyNames.Count < 2)
            throw new InternalDomainException(
                $"The chained rate limiter policy {config.PolicyName} must reference at least two policies, please check the app settings.",
                null);

        var chainedRateLimiterConfigs = new List<RateLimiterPolicyConfig>();
        foreach (var policyName in config.RateLimiterPolicyNames)
        {
            if (!policiesByName.TryGetValue(policyName, out var rateLimiterConfig))
                throw new InternalDomainException(
                    $"The chained rate limiter policy {config.PolicyName} references unknown policy {policyName}, please check the app settings.",
                    null);

            chainedRateLimiterConfigs.Add(rateLimiterConfig);
        }

        options.AddPolicy(config.PolicyName, context =>
            RateLimitPartition.Get(context.ResolveClientIpAddress(), _ =>
                RateLimiter.CreateChained(chainedRateLimiterConfigs.Select(CreateRateLimiter).ToArray())));
    }

    private static RateLimiter CreateRateLimiter(RateLimiterConfig config)
    {
        return config.RateLimiterType switch
        {
            nameof(FixedWindowRateLimiter) => new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = config.AutoReplenishment,
                PermitLimit = config.PermitLimit,
                QueueLimit = config.QueueLimit,
                QueueProcessingOrder = (QueueProcessingOrder)config.QueueProcessingOrder,
                Window = TimeSpan.FromSeconds(config.Window)
            }),
            nameof(SlidingWindowRateLimiter) => new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
            {
                AutoReplenishment = config.AutoReplenishment,
                PermitLimit = config.PermitLimit,
                QueueLimit = config.QueueLimit,
                QueueProcessingOrder = (QueueProcessingOrder)config.QueueProcessingOrder,
                Window = TimeSpan.FromSeconds(config.Window),
                SegmentsPerWindow = config.SegmentsPerWindow
            }),
            nameof(ConcurrencyLimiter) => new ConcurrencyLimiter(new ConcurrencyLimiterOptions
            {
                PermitLimit = config.PermitLimit,
                QueueLimit = config.QueueLimit,
                QueueProcessingOrder = (QueueProcessingOrder)config.QueueProcessingOrder
            }),
            nameof(TokenBucketRateLimiter) => new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
            {
                AutoReplenishment = config.AutoReplenishment,
                QueueLimit = config.QueueLimit,
                QueueProcessingOrder = (QueueProcessingOrder)config.QueueProcessingOrder,
                ReplenishmentPeriod = TimeSpan.FromSeconds(config.ReplenishmentPeriod),
                TokenLimit = config.TokenLimit,
                TokensPerPeriod = config.TokensPerPeriod
            }),
            _ => throw new InternalDomainException(
                $"The rate limiter type {config.RateLimiterType} is unknown, please check the app settings.", null)
        };
    }

    private static void ValidatePolicyNames(IEnumerable<IRateLimiterPolicyConfig> policies)
    {
        var duplicatePolicy = policies.GroupBy(policy => policy.PolicyName, StringComparer.Ordinal)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicatePolicy != null)
            throw new InternalDomainException(
                $"The rate limiter policy name {duplicatePolicy.Key} is duplicated, please check the app settings.", null);
    }

    private static void ValidatePolicyName(string policyName)
    {
        if (string.IsNullOrWhiteSpace(policyName))
            throw new InternalDomainException("A rate limiter policy name is required, please check the app settings.", null);
    }

    private static void ValidateRateLimiterConfig(RateLimiterConfig config, string policyName)
    {
        if (config.PermitLimit <= 0 || config.QueueLimit < 0 ||
            !Enum.IsDefined((QueueProcessingOrder)config.QueueProcessingOrder))
            throw new InternalDomainException(
                $"The rate limiter policy {policyName} has invalid permit, queue, or queue processing settings, please check the app settings.",
                null);

        if (config.RateLimiterType is nameof(FixedWindowRateLimiter) or nameof(SlidingWindowRateLimiter) &&
            config.Window <= 0)
            throw new InternalDomainException(
                $"The rate limiter policy {policyName} must have a positive window, please check the app settings.", null);

        if (config.RateLimiterType == nameof(SlidingWindowRateLimiter) && config.SegmentsPerWindow <= 0)
            throw new InternalDomainException(
                $"The sliding window policy {policyName} must have at least one segment, please check the app settings.", null);

        if (config.RateLimiterType == nameof(TokenBucketRateLimiter) &&
            (config.ReplenishmentPeriod <= 0 || config.TokenLimit <= 0 || config.TokensPerPeriod <= 0))
            throw new InternalDomainException(
                $"The token bucket policy {policyName} has invalid token settings, please check the app settings.", null);
    }
}