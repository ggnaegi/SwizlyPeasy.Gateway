using IdentityModel.Client;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SwizlyPeasy.Common;
using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Gateway.Mediator.handler;

public class LogoutHandler : IRequestHandler<LogoutRequest, Unit>
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IDiscoveryCache _discoveryCache;
    private readonly ILogger<LoginHandler> _logger;

    public LogoutHandler(
        IDiscoveryCache discoveryCache,
        IHttpContextAccessor contextAccessor,
        ILogger<LoginHandler> logger)
    {
        _logger = logger;
        _discoveryCache = discoveryCache;
        _contextAccessor = contextAccessor;
    }

    public async Task<Unit> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        //http context can't be null
        if (_contextAccessor.HttpContext == null) throw new InternalDomainException("Http context is null", null);
        _logger.LogInformation("Logout for ip {RemoteIpAddress}.",
            _contextAccessor.HttpContext.Connection.RemoteIpAddress);

        var siteCookies =
            _contextAccessor.HttpContext.Request.Cookies.Where(
                c => c.Key.StartsWith(Constants.Cookie));
        foreach (var cookie in siteCookies) _contextAccessor.HttpContext.Response.Cookies.Delete(cookie.Key);

        await _contextAccessor.HttpContext.SignOutAsync(Constants.CookiesAuthenticationProviderKey);

        //revoke token
        var discoveryDocument = await _discoveryCache.GetAsync();

        var tokenEndpoint = discoveryDocument.TokenEndpoint;
        var revocationEndpoint = discoveryDocument.RevocationEndpoint;

        _logger.LogWarning("Token Endpoint: {tokenEndpoint}, Revocation Endpoint: {revocationEndpoint}.", tokenEndpoint,
            revocationEndpoint);

        return Unit.Value;
    }
}