using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SwizlyPeasy.Common;
using SwizlyPeasy.Common.Dtos;

namespace SwizlyPeasy.Gateway.Controllers;

/// <summary>
///     Authorization controller
/// </summary>
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IOptions<AuthRedirectionConfig> _redirectOptions;

    /// <summary>
    /// </summary>
    /// <param name="redirectOptions"></param>
    public AuthController(IOptions<AuthRedirectionConfig> redirectOptions)
    {
        _redirectOptions = redirectOptions ?? throw new ArgumentNullException(nameof(redirectOptions));
    }

    /// <summary>
    ///     Convenience method for login, redirecting user to a dashboard or another front-end url
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet("/login")]
    public Task<ActionResult> Login()
    {
        return _redirectOptions.Value.MainUrl == null
            ? Task.FromResult<ActionResult>(Ok("Logged in..."))
            : Task.FromResult<ActionResult>(Redirect(_redirectOptions.Value.MainUrl));
    }

    /// <summary>
    ///     Convenience method for user logout, revoking tokens
    ///     redirecting the user after the logout process
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("/logout")]
    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync(Constants.CookiesAuthenticationProviderKey);
        await HttpContext.SignOutAsync(Constants.OpenIdConnect);

        return _redirectOptions.Value.IdpLogoutUrl == null
            ? Ok("Logged out...")
            : Redirect(_redirectOptions.Value.IdpLogoutUrl);
    }
}