namespace SwizlyPeasy.Common.Dtos;

/// <summary>
///     Open Id Connect configuration section.
///     By default, a demo server "demo.duendesoftware.com" (IdentityServer) is used as the IDP.
/// </summary>
public class OidcConfig
{
    public int RefreshTokenExpirationInHours { get; set; } = 1;
    public int RefreshThresholdMinutes { get; set; } = 1;
    public string[] Origins { get; set; } = Array.Empty<string>();
    public string Authority { get; set; } = "https://demo.duendesoftware.com/";
    public string CallbackUri { get; set; } = "/signin-oidc";
    public string ClientId { get; set; } = "interactive.confidential.short";
    public string ClientSecret { get; set; } = "secret";
    public string RedirectUri { get; set; } = "";
    public string[] Scopes { get; set; } = { "openid", "profile", "email", "offline_access" };
}