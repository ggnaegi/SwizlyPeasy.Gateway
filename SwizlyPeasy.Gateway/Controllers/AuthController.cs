using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SwizlyPeasy.Common.Dtos;
using SwizlyPeasy.Gateway.Mediator;

namespace SwizlyPeasy.Gateway.Controllers;

/// <summary>
///     Authorization controller
/// </summary>
[Route("login")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IOptions<AuthRedirectionConfig> _redirectOptions;

    /// <summary>
    /// </summary>
    /// <param name="mediator"></param>
    /// <param name="redirectOptions"></param>
    public AuthController(IMediator mediator, IOptions<AuthRedirectionConfig> redirectOptions)
    {
        _redirectOptions = redirectOptions ?? throw new ArgumentNullException(nameof(redirectOptions));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    ///     Convenience method for login, redirecting user to login page
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    public async Task<ActionResult> Login()
    {
        var userInfo = await _mediator.Send(new LoginRequest());
        if (_redirectOptions.Value.MainUrl == null) return Ok(userInfo);

        return Redirect(_redirectOptions.Value.MainUrl);
    }

    [AllowAnonymous]
    [HttpGet("/logout")]
    public async Task<ActionResult> Logout()
    {
        await _mediator.Send(new LogoutRequest());
        if (_redirectOptions.Value.IdpLogoutUrl == null) return Ok("logged out.");

        return Redirect(_redirectOptions.Value.IdpLogoutUrl);
    }
}