# SwizlyPeasy.Gateway
SwizlyPeasy.Gateway is a small API gateway project. This gateway should support OIDC authentication and service discovery with Consul. The reverse proxy is YARP.

- A user should be able to login, according to oidc scheme, and then redirected to a main page. (Extension methods for oidc are provided in SwizlyPeasy.Common.Auth)
- The tokens (tokens stored, encrypted in several cookies) must be checked for every request. If check successful, then the email and sub are forwarded to the microservices as headers. The authorization handled by the microservices themselves
- The microservices are registered to consul. The gateway retrieve them by using the service names (using "MetaData" in yarp config file)
  
https://microsoft.github.io/reverse-proxy/articles/config-files.html

https://microsoft.github.io/reverse-proxy/articles/authn-authz.html
