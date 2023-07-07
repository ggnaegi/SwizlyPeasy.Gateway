# SwizlyPeasy.Gateway
SwizlyPeasy.Gateway is a small API gateway based on YARP Reverse Proxy. This gateway should support OIDC authentication and service discovery with Consul.

## Introduction
Currently, YARP is the most advanced reverse proxy in .NET. The version 1 of this proxy was introduced by Microsoft at the end of 2021 (on my birthday...). Until now, I have used various API Gateways, including Ocelot, but the product hasn't evolved for some time. That's why I wondered if it was possible to graft the minimal functionalities to propose an API Gateway based on YARP.

The solution proposed here, "SwizlyPeasy.Gateway", is for now a PoC (Proof of Concept), but who knows, the results are promising so far, maybe I will be able to propose something more robust later on.

## Requirements
- The user must be able to authenticate using OpenID Connect.
- Two convenience endpoints for login/logout allow the user's redirection if the action was carried out correctly.
- Claims are transmitted to the microservices as headers. The microservices use the headers information for user authorization.
- The microservices are registered to consul. The gateway retrieve their addresses by using the service names.
- The cluster configuration is populated automatically, using service data retrieved from consul.
- The routes configuration is stored in consul KV store


  
https://microsoft.github.io/reverse-proxy/articles/config-files.html
https://microsoft.github.io/reverse-proxy/articles/authn-authz.html
