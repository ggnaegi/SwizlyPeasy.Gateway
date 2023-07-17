using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using SwizlyPeasy.Common.Dtos;

namespace SwizlyPeasy.Common.Middlewares;

/// <summary>
///     Middleware used to map headers transmitted by SwizlyPeasy.Gateway to claims
/// </summary>
public class HeaderToClaimsMiddleware
{
    private readonly IConfiguration _config;
    private readonly RequestDelegate _next;

    /// <summary>
    /// </summary>
    /// <param name="next"></param>
    /// <param name="config"></param>
    public HeaderToClaimsMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    ///     We are expecting the following headers from SwizlyPeasy.Gateway
    ///     "sub" & "email".
    ///     Of course, it is possible to add more headers. this can be modified using the claims config dto
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext httpContext)
    {
        var claimsConfig = new ClaimsConfig();
        _config.GetSection(Constants.ClaimsConfigSection).Bind(claimsConfig);
        var matchingClaims = httpContext.Request.Headers.Where(x => x.Key.Contains(claimsConfig.ClaimsHeaderPrefix));

        var claims = new List<Claim>();
        foreach (var matchingClaim in matchingClaims)
        {
            var claimType = matchingClaim.Key[(claimsConfig.ClaimsHeaderPrefix.Length + 1)..];
            AddToClaims(claims, claimType, matchingClaim.Value, claimsConfig);
        }

        await httpContext.SignInAsync(
            new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
        await _next(httpContext);
    }

    //An identity can contain multiple claims with multiple values and can contain multiple claims of the same type.
    private static void AddToClaims(IList<Claim> claims, string claimType, StringValues values, ClaimsConfig claimsConfig)
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