## SwizlyPeasy.Common

This package is part of SwizlyPeasy.Gateway project.
In this package, some extension methods are provided for OIDC configuration, Health checks configuration and RFC7807 exception handling configuration
- OIDC configuration

To configure OIDC, you should use the extension method ``` AddSwizlyPeasyOpenIdConnect(this IServiceCollection services, IConfiguration configuration) ```
in ``` program.cs ``` like this ``` services.AddSwizlyPeasyOpenIdConnect(configuration); ```

Without any settings set in appsettings, the default values will be loaded, configuring OIDC for a demo IDP (duendesoftware).

Below you can find the OIDC configuration parameters (in appsettings).
``` json
"OidcConfig": {
    "RefreshThresholdMinutes": 1,
    "Origins": [],
    "Authority": "https://demo.duendesoftware.com/",
    "CallbackUri": "/signin-oidc",
    "ClientId": "interactive.confidential.short",
    "ClientSecret": "secret",
    "RedirectUri": "",
    "Scopes": [ "openid", "profile", "email", "offline_access" ]
  }
```
- Health checks

Extension method configuring simple health checks, by default returning health check when calling ``` /health ``` endpoint. 

``` services.AddSwizlyPeasyHealthChecks(configuration); ```

- Exceptions Middleware

Middleware for exception handling, formating exceptions as defined by standard RFC7807, also handling 404 as an Exception.

``` 
public static void UseSwizlyPeasyExceptions(this WebApplication app)
    {
        app.UseMiddleware<ExceptionsHandlerMiddleware>();
        app.Use404AsException();
    }
```

``` app.UseSwizlyPeasyExceptions(); ```

For more information, please check the project SwizlyPeasy.Demo.API on github (https://github.com/ggnaegi/SwizlyPeasy.Gateway/tree/master/Demo/SwizlyPeasy.Demo.API)
