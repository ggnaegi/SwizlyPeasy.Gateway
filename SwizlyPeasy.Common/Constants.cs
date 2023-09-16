using Microsoft.Extensions.Primitives;

namespace SwizlyPeasy.Common;

/// <summary>
///     Some constants used in the application
/// </summary>
public static class Constants
{
    public const string OpenIdConnect = "SwizlyPeasyOidc";
    public const string CorsPolicy = "SwizlyPeasyCors";
    public const string Cookie = "SwizlyPeasyCookie";
    public const string CookiesAuthenticationProviderKey = "CookiesAuthentication";
    public const string OidcConfigSection = "OidcConfig";
    public const string AuthRedirectionConfigSection = "AuthRedirectionConfig";
    public const string ServiceDiscoveryConfigSection = "ServiceDiscovery";
    public const string ServiceRegistrationConfigSection = "ServiceRegistration";
    public const string ClaimsConfigSection = "ClaimsConfig";
    public const string RateLimiterPoliciesSection = "RateLimiterPolicies";
    public const string ProxiesAndNetworksConfigSection = "ProxiesAndNetworksConfig";
    public const string OidcPolicy = "oidc";
    public const string SubClaim = "sub";
    public const string EmailClaim = "email";
    public const string ChainedRateLimiter = "ChainedRateLimiter";
}