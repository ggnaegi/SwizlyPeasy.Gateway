namespace SwizlyPeasy.Common.HealthChecks
{
    /// <summary>
    /// </summary>
    public class HealthCheckResponse
    {
        /// <summary>
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// </summary>
        public IEnumerable<HealthCheckResponseItem>? HealthChecks { get; set; }

        /// <summary>
        /// </summary>
        public TimeSpan HealthCheckDuration { get; set; }
    }
}