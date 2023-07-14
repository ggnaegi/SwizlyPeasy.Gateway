using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

namespace SwizlyPeasy.Common.Auth;

public static class ClientAuthExtensions
{
    /// <summary>
    ///     Setting "dummy" authentication
    /// </summary>
    /// <param name="services"></param>
    public static void SetSwizlyPeasyAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Events.OnRedirectToAccessDenied = UnAuthorizedResponse;
                options.Events.OnRedirectToLogin = UnAuthorizedResponse;
            });
    }

    private static Task UnAuthorizedResponse(RedirectContext<CookieAuthenticationOptions> context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        return Task.CompletedTask;
    }
}