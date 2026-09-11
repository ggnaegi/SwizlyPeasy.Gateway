using System.Threading.RateLimiting;

namespace SwizlyPeasy.Common.Dtos;

public interface IRateLimiterPolicyConfig
{
    string PolicyName { get; }
}

public class RateLimiterConfig
{
    public string RateLimiterType { get; set; } = nameof(FixedWindowRateLimiter);
    public int PermitLimit { get; set; } = 100;
    public int Window { get; set; } = 10;
    public int ReplenishmentPeriod { get; set; } = 2;
    public int QueueLimit { get; set; } = 2;
    public int QueueProcessingOrder { get; set; } = 0;
    public int SegmentsPerWindow { get; set; } = 8;
    public int TokenLimit { get; set; } = 10;
    public int TokensPerPeriod { get; set; } = 4;
    public bool AutoReplenishment { get; set; } = false;
}

public class RateLimiterPolicyConfig : RateLimiterConfig, IRateLimiterPolicyConfig
{
    public string PolicyName { get; set; } = "DefaultPolicy";
}

public class ChainedRateLimiterPolicyConfig : IRateLimiterPolicyConfig
{
    public string PolicyName { get; set; } = "DefaultPolicy";
    public List<string> RateLimiterPolicyNames { get; set; } = [];
}