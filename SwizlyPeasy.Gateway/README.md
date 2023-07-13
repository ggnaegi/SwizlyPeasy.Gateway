## SwizlyPeasy.Gateway

This package is part of SwizlyPeasy.Gateway project. This package contains extension methods for SwizlyPeasy.Gateway configuration.

What is SwizlyPeasy.Gateway?

SwizlyPeasy.Gateway is a small API gateway based on YARP Reverse Proxy. This gateway should support OIDC authentication and service discovery with Consul.

## Introduction
Currently, YARP is the most advanced reverse proxy in .NET. The version 1 of this proxy was introduced by Microsoft at the end of 2021. I wondered if it was possible to graft the minimal functionalities to propose an API Gateway based on YARP.
https://devblogs.microsoft.com/dotnet/announcing-yarp-1-0-release/

about consul: https://developer.hashicorp.com/consul/download

The solution proposed here, "SwizlyPeasy.Gateway", is for now a PoC (Proof of Concept), but who knows, the results are promising so far, maybe I will be able to propose something more robust later on.

## Requirements
- The user must be able to authenticate using OpenID Connect.
- Two convenience endpoints for login/logout allow the user's redirection if the action was carried out correctly.
- Claims are transmitted to the microservices as headers. The microservices use the headers information for user authorization.
- The microservices are registered to consul. The gateway retrieve their addresses by using the service names.
- The cluster configuration is populated automatically, using service data retrieved from consul.
- The routes configuration is stored in consul KV store

## Setup
The setup is straight forward
- Use the provided extension methods in ```program.cs``` 

```
var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSwizlyPeasyGateway(builder.Configuration);

    var app = builder.Build();

    app.UseSwizlyPeasyGateway();
    app.Run();
```

- Provide a ```routes.config.json``` file (please have a look at the SwizlyPeasy.Gateway.API demo project).
The syntax is the same as YARP configuration for routes.
```
{
  "Routes": {
    "route1": {
      "ClusterId": "DemoAPI",
      "AuthorizationPolicy": "oidc",
      "Match": {
        "Path": "/api/v1/demo/weather"
      },
      "Transforms": [
        {
          "RequestHeader": "Accept-Language",
          "Set": "de-CH"
        }
      ]
    },
    "route2": {
      "ClusterId": "DemoAPI",
      "AuthorizationPolicy": "oidc",
      "Match": {
        "Path": "/api/v1/demo/weather-with-authorization"
      },
      "Transforms": [
        {
          "RequestHeader": "Accept-Language",
          "Set": "de-CH"
        }
      ]
    }
  }
}
```
- Define your configuration in appsettings
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "OidcConfig": {
    "RefreshThresholdMinutes": 1,
    "Origins": [],
    "Authority": "https://demo.duendesoftware.com/",
    "CallbackUri": "/signin-oidc",
    "ClientId": "interactive.confidential.short",
    "ClientSecret": "secret",
    "RedirectUri": "",
    "Scopes": [ "openid", "profile", "email", "offline_access" ]
  },
  "ServiceDiscovery": {
    "Scheme": "http",
    "RefreshIntervalInSeconds": 120,
    "LoadBalancingPolicy": "Random",
    "KeyValueStoreKey": "SwizlyPeasy.Gateway",
    "ServiceDiscoveryAddress": "http://consul:8500"
  }
}
```