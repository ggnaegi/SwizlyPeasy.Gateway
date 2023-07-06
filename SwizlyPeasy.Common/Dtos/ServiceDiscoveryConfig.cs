namespace SwizlyPeasy.Common.Dtos
{
    public class ServiceDiscoveryConfig
    {
        public string Scheme { get; set; } = "http";
        public int RefreshIntervalInSeconds { get; set; } = 120;
        public string LoadBalancingPolicy { get; set; } = "Random";
        public string KeyValueStoreKey { get; set; } = "SwizlyPeasy.Gateway";
        /// <summary>
        ///     consul service address
        /// </summary>
        public Uri ServiceDiscoveryAddress { get; set; } = new Uri("http://localhost:8500");

        /// <summary>
        ///     current service address
        /// </summary>
        public Uri ServiceAddress { get; set; } = new Uri("http://localhost:8001");

        /// <summary>
        ///     current service name
        /// </summary>
        public string ServiceName { get; set; } = "SwizlyPeasy.Gateway";

        /// <summary>
        ///     service's instance Id (load balancing)
        /// </summary>
        public string ServiceId { get; set; } = "1";
    }
}
