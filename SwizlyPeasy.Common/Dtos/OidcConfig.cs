using Microsoft.AspNetCore.Http;

namespace SwizlyPeasy.Common.Dtos;

/// <summary>
///     Open Id Connect configuration section.
///     By default, a demo server "demo.duendesoftware.com" (IdentityServer) is used as the IDP.
/// </summary>
public class OidcConfig
{
    public int RefreshTokenExpirationInHours { get; set; } = 1;
    public SameSiteMode MinimumSameSiteMode { get; set; } = SameSiteMode.None;
    public int RefreshThresholdMinutes { get; set; } = 1;
    public bool AllowCors { get; set; }
    public string[] Origins { get; set; } = Array.Empty<string>();
    public string Authority { get; set; } = "";
    public string CallbackUri { get; set; } = "/signin-oidc";
    public string ClientId { get; set; } = "";
    public string ClientSecret { get; set; } = "";
    public string RedirectUri { get; set; } = "";
    public string[] Scopes { get; set; } = { "openid", "profile", "email", "offline_access" };
    public bool DisableOidc { get; set; } = false;
}