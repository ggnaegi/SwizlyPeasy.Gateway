namespace SwizlyPeasy.Common.HealthChecks
{
    /// <summary>
    /// </summary>
    public class HealthCheckResponseItem
    {
        /// <summary>
        ///     Current health check status (healthy/unhealthy)
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        ///     current component (eg. database context)
        /// </summary>
        public string? Components { get; set; }

        /// <summary>
        ///     If available, returning current exception message
        /// </summary>
        public string? ExceptionMessage { get; set; }

        /// <summary>
        ///     Current health check description
        /// </summary>
        public string? Description { get; set; }
    }
}