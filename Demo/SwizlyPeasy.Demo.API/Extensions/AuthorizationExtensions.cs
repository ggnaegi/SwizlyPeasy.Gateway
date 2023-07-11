using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using SwizlyPeasy.Demo.API.Authorization;

namespace SwizlyPeasy.Demo.API.Extensions;

public static class AuthorizationExtensions
{
    /// <summary>
    ///     Adding demo authorization policy, checking if user's uid/sub matches bob's one
    /// </summary>
    /// <param name="services"></param>
    public static void SetAuthenticationAndAuthorization(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Events.OnRedirectToAccessDenied = UnAuthorizedResponse;
                options.Events.OnRedirectToLogin = UnAuthorizedResponse;
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AreYouBob", policy =>
                policy.Requirements.Add(new BobRequirement()));
        });
        services.AddSingleton<IAuthorizationHandler, AppAuthorizationHandler>();
    }

    private static Task UnAuthorizedResponse(RedirectContext<CookieAuthenticationOptions> context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        return Task.CompletedTask;
    }
}