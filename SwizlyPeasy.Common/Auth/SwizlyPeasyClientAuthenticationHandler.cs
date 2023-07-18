using System.Security.Claims;
using System.Text.Encodings.Web;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using SwizlyPeasy.Common.Dtos;

namespace SwizlyPeasy.Common.Auth;

public class SwizlyPeasyClientAuthenticationHandler : AuthenticationHandler<SwizlyPeasyAuthenticationOptions>
{
    public SwizlyPeasyClientAuthenticationHandler(IOptionsMonitor<SwizlyPeasyAuthenticationOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claimsConfig = Context.RequestServices.GetService<IOptions<ClaimsConfig>>();

        if (claimsConfig == null)
            return Task.FromResult(
                AuthenticateResult.Fail("Missing Claims configuration, must be defined in appsettings!"));

        if (!claimsConfig.Value.ClaimsAsHeaders.Contains(JwtClaimTypes.Subject))
            return Task.FromResult(
                AuthenticateResult.Fail("If using authentication, then claims config must contain Subject claim key!"));

        var subjectHeaderKey = $"{claimsConfig.Value.ClaimsHeaderPrefix}-{JwtClaimTypes.Subject}";

        if (!Request.Headers.ContainsKey(subjectHeaderKey))
            return Task.FromResult(AuthenticateResult.NoResult());

        var matchingClaims = Request.Headers.Where(x => x.Key.Contains(claimsConfig.Value.ClaimsHeaderPrefix));

        var claims = new List<Claim>();
        foreach (var matchingClaim in matchingClaims)
        {
            var claimType = matchingClaim.Key[(claimsConfig.Value.ClaimsHeaderPrefix.Length + 1)..];
            AddToClaims(claims, claimType, matchingClaim.Value, claimsConfig.Value);
        }

        var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
    }

    //An identity can contain multiple claims with multiple values and can contain multiple claims of the same type.
    private static void AddToClaims(ICollection<Claim> claims, string claimType, StringValues values,
        ClaimsConfig claimsConfig)
    {
        foreach (var value in values)
        {
            if (value == null) continue;

            if (claimsConfig.JwtToIdentityClaimsMappings.TryGetValue(claimType, out var alternateClaim))
                claims.Add(new Claim(alternateClaim, value));

            claims.Add(new Claim(claimType, value));
        }
    }
}