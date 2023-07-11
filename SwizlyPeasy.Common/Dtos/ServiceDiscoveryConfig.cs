namespace SwizlyPeasy.Common.Dtos;

/// <summary>
///     Consul service discovery config
///     By default using localhost:8500
/// </summary>
public class ServiceDiscoveryConfig
{
    /// <summary>
    ///     Scheme, could be http or https
    /// </summary>
    public string Scheme { get; set; } = "http";

    /// <summary>
    ///     Refresh interval of the list of services referenced in consul.
    /// </summary>
    public int RefreshIntervalInSeconds { get; set; } = 120;

    /// <summary>
    ///     The load Balancing policy, by default "Random",
    ///     could be: "FirstAlphabetical", "PowerOfTwoChoices", "RoundRobin", "LeastRequests"
    ///     https://microsoft.github.io/reverse-proxy/articles/load-balancing.html
    /// </summary>
    public string LoadBalancingPolicy { get; set; } = "Random";

    /// <summary>
    ///     Key used for routes configuration data in consul
    ///     KV store
    ///     https://developer.hashicorp.com/consul/docs/dynamic-app-config/kv
    /// </summary>
    public string KeyValueStoreKey { get; set; } = "SwizlyPeasy.Gateway";

    /// <summary>
    ///     consul service address
    /// </summary>
    public Uri ServiceDiscoveryAddress { get; set; } = new("http://localhost:8500");
}