## SwizlyPeasy.Consul

This package is part of SwizlyPeasy.Gateway project.
In this package, some extension methods are provided for consul client configuration and service registration in consul. 

This package contains an interesting extension method, configuring the automatic service registration to consul. 

By using 
```
services.ConfigureConsulClient(configuration);
services.RegisterService(configuration);
```
in ```program.cs``` your API will be registered to your consul instance.

The default configuration (appsettings) looks like this:
```
"ServiceDiscovery": {
    "Scheme": "http",
    "RefreshIntervalInSeconds": 120,
    "LoadBalancingPolicy": "Random",
    "KeyValueStoreKey": "SwizlyPeasy.Gateway",
    "ServiceDiscoveryAddress": "http://localhost:8500"
  },
  "ServiceRegistration": {
    "ServiceName": "DemoAPI",
    "ServiceId": "1",
    "ServiceAddress": "http://localhost:8020",
    "HealthCheckPath": "health"
  }
```
The parameters in Service Discovery part are irrelevant for a client API, except for the server address (ServiceDiscoveryAddress).

- ServiceName: The service type name (the services will be grouped by service name)
- ServiceId: The service id (for load balancing) -> Key is then "DemoAPI-1"
- ServiceAddress: The current service address
- HealthCheckPath: The health endpoint path (you could use the health extension methods in SwizlyPeasy.Common)
