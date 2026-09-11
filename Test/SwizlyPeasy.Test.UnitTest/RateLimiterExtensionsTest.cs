using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SwizlyPeasy.Common.Exceptions;
using SwizlyPeasy.Common.Extensions;

namespace SwizlyPeasy.Test.UnitTest;

public class RateLimiterExtensionsTest
{
    [Fact]
    public void AddSwizlyPeasyRateLimiters_ValidChainedPolicy_ConfiguresRateLimiting()
    {
        using var serviceProvider = CreateServiceProvider(new Dictionary<string, string?>
        {
            ["ChainedRateLimiterPolicies:0:PolicyName"] = "combined",
            ["ChainedRateLimiterPolicies:0:RateLimiterConfigs:0:RateLimiterType"] = "FixedWindowRateLimiter",
            ["ChainedRateLimiterPolicies:0:RateLimiterConfigs:0:PermitLimit"] = "10",
            ["ChainedRateLimiterPolicies:0:RateLimiterConfigs:0:Window"] = "60",
            ["ChainedRateLimiterPolicies:0:RateLimiterConfigs:1:RateLimiterType"] = "ConcurrencyLimiter",
            ["ChainedRateLimiterPolicies:0:RateLimiterConfigs:1:PermitLimit"] = "5"
        });

        var options = serviceProvider.GetRequiredService<IOptions<RateLimiterOptions>>().Value;

        Assert.NotNull(options);
    }

    [Fact]
    public void AddSwizlyPeasyRateLimiters_ChainedPolicyWithOneLimiter_ThrowsConfigurationException()
    {
        using var serviceProvider = CreateServiceProvider(new Dictionary<string, string?>
        {
            ["ChainedRateLimiterPolicies:0:PolicyName"] = "invalid-combined",
            ["ChainedRateLimiterPolicies:0:RateLimiterConfigs:0:RateLimiterType"] = "FixedWindowRateLimiter",
            ["ChainedRateLimiterPolicies:0:RateLimiterConfigs:0:PermitLimit"] = "10",
            ["ChainedRateLimiterPolicies:0:RateLimiterConfigs:0:Window"] = "60"
        });

        Assert.Throws<InternalDomainException>(
            () => serviceProvider.GetRequiredService<IOptions<RateLimiterOptions>>().Value);
    }

    private static ServiceProvider CreateServiceProvider(IReadOnlyDictionary<string, string?> settings)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        var services = new ServiceCollection();
        services.AddSwizlyPeasyRateLimiters(configuration);

        return services.BuildServiceProvider();
    }
}
