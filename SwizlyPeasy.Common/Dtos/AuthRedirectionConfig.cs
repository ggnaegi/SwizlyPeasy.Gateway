namespace SwizlyPeasy.Common.Dtos
{
    /// <summary>
    /// It is possible to define redirection paths
    /// for the "/login" and "/logout" convenience endpoints.
    /// We could redirect the user to the frontend main page.
    /// </summary>
    public class AuthRedirectionConfig
    {
        public string? MainUrl { get; set; }
        public string? IdpLogoutUrl { get; set; }
    }
}
