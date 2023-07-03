namespace SwizlyPeasy.Common.Dtos
{
    public class ServiceDiscoveryConfigDto
    {
        /// <summary>
        ///     consul service address
        /// </summary>
        public Uri? ServiceDiscoveryAddress { get; set; }

        /// <summary>
        ///     current service address
        /// </summary>
        public Uri? ServiceAddress { get; set; }

        /// <summary>
        ///     current service name
        /// </summary>
        public string? ServiceName { get; set; }

        /// <summary>
        ///     service's instance Id (load balancing)
        /// </summary>
        public string? ServiceId { get; set; }
    }
}
