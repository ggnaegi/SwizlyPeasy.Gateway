# SwizlyPeasy.Gateway
SwizlyPeasy.Gateway is a small API gateway based on YARP Reverse Proxy. This gateway should support OIDC authentication and service discovery with Consul.

## Introduction
Currently, YARP is the most advanced reverse proxy in .NET. The version 1 of this proxy was introduced by Microsoft at the end of 2021 (on my birthday...). Until now, I have used various API Gateways, including Ocelot, but the product hasn't evolved for some time. That's why I wondered if it was possible to graft the minimal functionalities to propose an API Gateway based on YARP.
https://devblogs.microsoft.com/dotnet/announcing-yarp-1-0-release/

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
- SwizlyPeasy.Consul: In this part of the solution, we offer various extension methods for configuring the Consul client, obtaining the list of services, managing data from the KV store, but also for registering a service in Consul.
- SwizlyPeasy.Gateway: This is the gateway itself. We notice the presence of a controller containing two convenience endpoints for the user authentication (login/logout). There is also the "routes.config.json" file which contains the routes configuration according to the YARP syntax. This file is automatically loaded into the Consul KV store when the application starts. The configuration can then be modified on the fly (the configuration is updated at regular intervals). The Cluster part of the configuration is automatically generated using the information provided by Consul.

On this topic, thanks to Layla Porter: https://tanzu.vmware.com/developer/blog/build-api-gateway-csharp-yarp-eureka/ The approach is nevertheless different as I propose a modification of the routes on the fly using the KV store, and the authentication with OIDC is also implemented.

## Starting the demo
This is a demonstration, it's clear that several security measures need to be taken in a production environment...

### Consul
https://developer.hashicorp.com/consul/downloads

-> Unzip the file in a folder and start consul (windows, using powershell): ```.\consul.exe agent -dev ``` 

-> The Consul instance can now be reached at the address http://localhost:8500.

![image](https://github.com/ggnaegi/SwizlyPeasy.Gateway/assets/58469901/408b55df-9676-466d-952f-c6b6285a3d4d)
### Start the SwizlyPeasy.Gateway and the Swizly.Demo.API projects

You should clone the repository, open the solution in visual studio and start the Gateway and Demo Projects (Multiple Startup Projects) in debug mode with IIS Express
![image](https://github.com/ggnaegi/SwizlyPeasy.Gateway/assets/58469901/d7b80a8f-cded-4703-92e9-8fcfb9cf00a8)

You should see this when starting the gateway...
![image](https://github.com/ggnaegi/SwizlyPeasy.Gateway/assets/58469901/50cea2ea-8d50-4151-a739-50456b93a19c)

Now, you could try the two routes configured in YARP:

```http://localhost:44369/api/v1/demo/weather```

```http://localhost:44369/api/v1/demo/weather-with-authorization```

You must be authenticated to access these two paths, so the system will ask you to authenticate. You should see the duende IdentityServer demo page. 

![image](https://github.com/ggnaegi/SwizlyPeasy.Gateway/assets/58469901/4a840a57-6ab5-4cf4-8ee6-5c0c866aecaf)

Now you can choose between bob, alice, or use your google account. For the demo purpose you should please use alice...

![image](https://github.com/ggnaegi/SwizlyPeasy.Gateway/assets/58469901/ab672d25-5f6e-4b36-95d4-731e6c47663e)

And with weather-with-authorization...

![image](https://github.com/ggnaegi/SwizlyPeasy.Gateway/assets/58469901/ac38b74a-f370-40a3-96b9-3ade9ccf6843)

You're obviously not Bob...


