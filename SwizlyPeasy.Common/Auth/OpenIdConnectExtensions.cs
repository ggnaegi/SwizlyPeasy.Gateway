using System.Net;
using System.Security.Claims;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SwizlyPeasy.Common.Dtos;
using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Common.Auth;

public static class OpenIdConnectExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private static Task UnAuthorizedResponse(RedirectContext<CookieAuthenticationOptions> context)
    {
        context.Response.Headers["Location"] = context.RedirectUri;
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Check if token has expired or not
    /// </summary>
    /// <param name="expiresAt"></param>
    /// <param name="refreshThresholdMinutes"></param>
    /// <returns></returns>
    private static bool HasExpired(string expiresAt, int refreshThresholdMinutes)
    {
        return DateTimeOffset
            .Parse(expiresAt)
            .Subtract(DateTimeOffset.UtcNow) < TimeSpan.FromMinutes(refreshThresholdMinutes);
    }

    /// <summary>
    ///     Extension method, adding open id connect
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddSwizlyPeasyOpenIdConnect(this IServiceCollection services, IConfiguration configuration)
    {
        var config = new OidcConfig();
        configuration.GetSection(Constants.OidcConfigSection).Bind(config);

        services.ConfigureDiscoveryCache(config);
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = Constants.CookiesAuthenticationProviderKey;
                options.DefaultSignInScheme = Constants.CookiesAuthenticationProviderKey;
                options.DefaultChallengeScheme = Constants.OpenIdConnect;
            })
            .ConfigureCookies(config)
            .ConfigureOpenIdConnect(config);

        services.ConfigureCors(config);
    }


    /// <summary>
    ///     Configuring cookies, for cors support
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    private static AuthenticationBuilder ConfigureCookies(this AuthenticationBuilder builder, OidcConfig config)
    {
        builder.AddCookie(Constants.CookiesAuthenticationProviderKey, options =>
        {
            options.Cookie.Name = Constants.Cookie;

            //using http only, same site = None, and secure cookies
            //so we can reuse the cookie created in the back end with
            //ajax or fetch, when front end and back end aren't in the 
            //same domain
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

            //the auth cookie is essential, 
            //it should be sent for most requests on the api
            options.Cookie.IsEssential = true;

            //Adding 24 hours expiration time
            options.ExpireTimeSpan = TimeSpan.FromHours(
                config.RefreshTokenExpirationInHours
            );
            options.SlidingExpiration = true;

            //we don't want any redirection, front end
            //will redirect the end user, so we return 401 if unauthorized, that's it
            options.Events.OnRedirectToAccessDenied = UnAuthorizedResponse;
            options.Events.OnRedirectToLogin = UnAuthorizedResponse;

            //Here we need to address the refresh token issue
            //since it's not working with standard methods or hasn't been implemented
            //yet.
            options.Events.OnValidatePrincipal = async context => await OnValidatePrincipal(context, config);
        });

        return builder;
    }

    /// <summary>
    ///     Configure cors, allowing some origins defined in app settings
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    private static void ConfigureCors(this IServiceCollection services, OidcConfig config)
    {
        //specify origins
        services.AddCors(options =>
        {
            options.AddPolicy(Constants.CorsPolicy,
                builder =>
                {
                    var origins = config.Origins;
                    builder.WithOrigins(origins)
                        .AllowCredentials()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });
    }

    /// <summary>
    ///     Configure discovery cache, avoiding getting the discovery data
    ///     for each request
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    private static void ConfigureDiscoveryCache(this IServiceCollection services, OidcConfig config)
    {
        //Adding discovery document cache
        //avoiding requesting discovery document for every api call
        services.AddSingleton<IDiscoveryCache>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            return new DiscoveryCache(config.Authority,
                () => factory.CreateClient());
        });
    }

    /// <summary>
    ///     Configuring open id connect
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    private static AuthenticationBuilder ConfigureOpenIdConnect(this AuthenticationBuilder builder, OidcConfig config)
    {
        builder.AddOpenIdConnect(Constants.OpenIdConnect, options =>
        {
            options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;

            // if response type is code, then PKCE is active
            options.ResponseType = "code";
            options.UseTokenLifetime = false;
            options.SaveTokens = true;

            //no need of redirect uri, it's defined using the callback path
            var callbackPath = config.CallbackUri;
            if (!string.IsNullOrEmpty(callbackPath)) options.CallbackPath = new PathString(callbackPath);

            options.ClientId = config.ClientId;
            options.ClientSecret = config.ClientSecret;
            options.Authority = config.Authority;

            options.Scope.Clear();
            //could add more scopes if supported by the oidc provider
            var scopes = config.Scopes;
            foreach (var scope in scopes) options.Scope.Add(scope);

            options.GetClaimsFromUserInfoEndpoint = true;

            //fix for production environment
            options.Events.OnRedirectToIdentityProvider = async n =>
            {
                var redirectUri = config.RedirectUri;
                if (!string.IsNullOrEmpty(redirectUri)) n.ProtocolMessage.RedirectUri = redirectUri;

                await Task.CompletedTask;
            };

            //options.Events.OnMessageReceived
            options.BackchannelHttpHandler = new HttpClientHandler { UseCookies = false };
        });

        return builder;
    }

    /// <summary>
    ///     Validating cookies, refreshing them when included jwt has expired
    /// </summary>
    /// <param name="context"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    private static async Task OnValidatePrincipal(CookieValidatePrincipalContext context, OidcConfig config)
    {
        if (HasExpired(context.Properties.GetTokenValue("expires_at") ?? throw new InvalidOperationException(),
                config.RefreshThresholdMinutes))
        {
            //getting discovery document, avoiding hard coding the token endpoint uri
            //somewhere, using discovery cache to improve performance
            //avoiding not needed calls to the discovery endpoint.
            var discoveryCache = context.HttpContext.RequestServices.GetService<IDiscoveryCache>();
            if (discoveryCache == null) throw new InternalDomainException("Discovery cache misconfiguration", null);

            var discoveryDocument = await discoveryCache.GetAsync();


            //creating new http client
            //with custom handler to avoid cookies usage
            var currentClient = new HttpClient(
                new HttpClientHandler { UseCookies = false }
            );

            //requesting new tokens using the refresh token
            var response = await currentClient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                RequestUri = new Uri(discoveryDocument.TokenEndpoint),
                ClientId = config.ClientId,
                ClientSecret = config.ClientSecret,
                GrantType = "refresh_token",
                RefreshToken = context.Properties.GetTokenValue("refresh_token")
            });


            if (!response.IsError)
            {
                var expiresInSeconds = response.ExpiresIn;
                var updatedExpiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds);

                //refresh token is a single usage token, so we have to update it as well.
                context.Properties.UpdateTokenValue("expires_at", updatedExpiresAt.ToString());
                context.Properties.UpdateTokenValue("access_token", response.AccessToken);
                context.Properties.UpdateTokenValue("refresh_token", response.RefreshToken);

                context.ShouldRenew = true;
            }
            else
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync();
            }
        }
    }

    /// <summary>
    ///     Using cross origin resource sharing services
    ///     allowing front end app to access resources
    /// </summary>
    /// <param name="app"></param>
    public static void UseSwizlyPeasyOidc(this IApplicationBuilder app)
    {
        app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.None,
            Secure = CookieSecurePolicy.Always,
            HttpOnly = HttpOnlyPolicy.Always
        });

        app.UseCors(Constants.CorsPolicy);
    }
}