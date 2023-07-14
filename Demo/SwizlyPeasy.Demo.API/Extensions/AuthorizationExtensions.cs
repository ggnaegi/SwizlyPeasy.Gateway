using Microsoft.AspNetCore.Authorization;
using SwizlyPeasy.Demo.API.Authorization;

namespace SwizlyPeasy.Demo.API.Extensions;

public static class AuthorizationExtensions
{
    /// <summary>
    ///     Test policy, checking if user is bob
    /// </summary>
    /// <param name="services"></param>
    public static void SetAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AreYouBob", policy =>
                policy.Requirements.Add(new BobRequirement()));
        });
        services.AddSingleton<IAuthorizationHandler, AppAuthorizationHandler>();
    }
}