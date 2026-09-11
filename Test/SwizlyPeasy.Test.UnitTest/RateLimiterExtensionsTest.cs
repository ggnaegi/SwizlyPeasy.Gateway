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
            ["RateLimiterPolicies:0:PolicyName"] = "burst",
            ["RateLimiterPolicies:0:RateLimiterType"] = "FixedWindowRateLimiter",
            ["RateLimiterPolicies:0:PermitLimit"] = "10",
            ["RateLimiterPolicies:0:Window"] = "60",
            ["RateLimiterPolicies:1:PolicyName"] = "concurrency",
            ["RateLimiterPolicies:1:RateLimiterType"] = "ConcurrencyLimiter",
            ["RateLimiterPolicies:1:PermitLimit"] = "5",
            ["ChainedRateLimiterPolicies:0:PolicyName"] = "combined",
            ["ChainedRateLimiterPolicies:0:RateLimiterPolicyNames:0"] = "burst",
            ["ChainedRateLimiterPolicies:0:RateLimiterPolicyNames:1"] = "concurrency"
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
            ["ChainedRateLimiterPolicies:0:RateLimiterPolicyNames:0"] = "burst"
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
