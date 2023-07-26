[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=ggnaegi_SwizlyPeasy.Gateway&metric=sqale_rating)](https://sonarcloud.io/project/overview?id=ggnaegi_SwizlyPeasy.Gateway)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=ggnaegi_SwizlyPeasy.Gateway&metric=reliability_rating)](https://sonarcloud.io/project/overview?id=ggnaegi_SwizlyPeasy.Gateway)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=ggnaegi_SwizlyPeasy.Gateway&metric=security_rating)](https://sonarcloud.io/project/overview?id=ggnaegi_SwizlyPeasy.Gateway)

# SwizlyPeasy.Gateway
SwizlyPeasy.Gateway is a small API gateway based on YARP Reverse Proxy. This gateway should support OIDC authentication and service discovery with Consul.

## Introduction
Currently, YARP is the most advanced reverse proxy in .NET. The version 1 of this proxy was introduced by Microsoft at the end of 2021 (on my birthday...). Until now, I have used various API Gateways, including Ocelot. I wondered if it was possible to graft the minimal functionalities to propose an API Gateway based on YARP.
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

## Structure
- Demo: In this folder, there is a very simple demonstration API - SwizlyPeasy.Demo.API - that allows testing of the service registration in Consul, authorization with policies, and the routes configuration in "SwizlyPeasy.Gateway".
- SwizlyPeasy.Common: Some cross cutting topics here, such as the Exceptions (compliant with RFC 7807), the OIDC Extension methods, some DTOs used at application level and the health checks.
- SwizlyPeasy.Consul: In this part of the solution, we offer various extension methods for configuring the Consul client, obtaining the list of services, managing data from the KV store, but also for registering a service in Consul or retrieving health checks
- SwizlyPeasy.Gateway: This is the gateway itself. We notice the presence of a controller containing two convenience endpoints for the user authentication (login/logout). There is also the "routes.config.json" file which contains the routes configuration according to the YARP syntax. This file is automatically loaded into the Consul KV store when the application starts. The configuration can then be modified on the fly (the configuration is updated at regular intervals). The Cluster part of the configuration is automatically generated using the information provided by Consul.
- SwizlyPeasy.Gateway.API: The projects Common, Consul and Gateway will be, in a near future, provided as Nuget Packages.This project has been created so that you can start the gateway from Visual Studio or elsewhere...

On this topic, thanks to Layla Porter: https://tanzu.vmware.com/developer/blog/build-api-gateway-csharp-yarp-eureka/ The approach is nevertheless different as I propose a modification of the routes on the fly using the KV store, and the authentication with OIDC is also implemented.

## Starting the demo
This is a demonstration, and it is clear that several security measures need to be taken in a production environment... But the demo has been improved. It can now be started with docker-compose, no need to download consul, it is provided as a docker container.
-> You should make sure that docker is installed...

But first, clone the repo, easy ```git clone https://github.com/ggnaegi/SwizlyPeasy.Gateway.git```

### Start Consul, SwizlyPeasy.Gateway.API and Swizly.Demo.API projects

#### In Visual Studio 2022
Open the solution in visual studio and start with docker-compose
![image](https://github.com/ggnaegi/SwizlyPeasy.Gateway/assets/58469901/3b9acd54-2793-4fbf-ba12-b06a3a451a95)

#### What if you don't want to use Visual Studio 2022...

Or open a powershell in the cloned folder...
![image](https://github.com/ggnaegi/SwizlyPeasy.Gateway/assets/58469901/3873d856-6f75-4c62-abfa-03ceeb9e5826)

And execute the following command:
```docker compose -f docker-compose.yml -f docker-compose.override.yml up```

#### Now you should be able to call the services...
- Consul service, http://localhost:8500
- SwizlyPeasy.Gateway.API, https://localhost:8001
![image](https://github.com/ggnaegi/SwizlyPeasy.Gateway/assets/58469901/50476fe0-2138-4b08-b6fd-26dc54c23f39)

#### Let's try the routes...

You could try the two routes configured in YARP. 

```https://localhost:8001/api/v1/demo/weather```

```https://localhost:8001/api/v1/demo/weather-with-authorization```

You must be authenticated to access these two paths, the system will ask you to authenticate. You should see the duende IdentityServer demo page. 

![image](https://github.com/ggnaegi/SwizlyPeasy.Gateway/assets/58469901/4a840a57-6ab5-4cf4-8ee6-5c0c866aecaf)

Now you can choose between bob, alice, or use your google account. For the demo purpose you should please use alice...

![image](https://github.com/ggnaegi/SwizlyPeasy.Gateway/assets/58469901/ab672d25-5f6e-4b36-95d4-731e6c47663e)

And with weather-with-authorization...

![image](https://github.com/ggnaegi/SwizlyPeasy.Gateway/assets/58469901/ac38b74a-f370-40a3-96b9-3ade9ccf6843)

You're obviously not Bob...

## Configuring the gateway

You should have a look at the SwizlyPeasy.Gateway.API project.

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
  },
  "ClaimsConfig": {
    "ClaimsHeaderPrefix": "SWIZLY-PEASY",
    "ClaimsAsHeaders": [
      "sub",
      "email",
      "name",
      "family_name"
    ],
    "JwtToIdentityClaimsMappings": {
      "sub": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
      "email": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
    }
  }
}
```

## Registering a client

You should have a look at the SwizlyPeasy.DEMO.API project.

Production: Make sure you can't reached the client from the web (encapsulated like in docker), otherwise all this makes no sense.

The setup is a bit more complicated than for the gateway itself, because of the middlewares ordering...

First, use the extension method ```RegisterServiceToSwizlyPeasyGateway```, this will configure the consul client, the service registration to consul and configure the health checks.
Then, add a "custom" authentication method using ```SetSwizlyPeasyAuthentication```, as the claims will be provided as headers.

MiddleWares:
- ```app.UseSwizlyPeasyExceptions();``` Handling exceptions and returning them in RFC 7807 format
- ```app.UseSwizlyPeasyHealthChecks();``` Formating the output from health endpoint

Example:
```
// swizly peasy consul & health checks
builder.Services.RegisterServiceToSwizlyPeasyGateway(builder.Configuration);
builder.Services.SetSwizlyPeasyAuthentication(builder.Configuration);
builder.Services.SetAuthorization();

...

var app = builder.Build();
app.UseSwizlyPeasyExceptions();

...

app.UseHttpsRedirection();
app.UseAuthentication();
//--------- Swizly Peasy MiddleWares ----------
// swizly peasy health checks middleware
app.UseSwizlyPeasyHealthChecks();
//---------------------------------------------
app.UseAuthorization();
app.MapControllers();
```

Configuration (appsettings):
```
"ServiceDiscovery": {
    "Scheme": "http",
    "RefreshIntervalInSeconds": 120,
    "LoadBalancingPolicy": "Random",
    "KeyValueStoreKey": "SwizlyPeasy.Gateway",
    "ServiceDiscoveryAddress": "http://consul:8500"
  },
  "ServiceRegistration": {
    "ServiceName": "DemoAPI",
    "ServiceId": "1",
    "ServiceAddress": "http://demo",
    "HealthCheckPath": "health"
  },
  "ClaimsConfig": {
    "ClaimsHeaderPrefix": "SWIZLY-PEASY",
    "ClaimsAsHeaders": [
      "sub",
      "email",
      "name",
      "family_name"
    ],
    "JwtToIdentityClaimsMappings": {
      "sub": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
      "email": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
    }
  }
}
```
