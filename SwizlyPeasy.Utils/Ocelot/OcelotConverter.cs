using Newtonsoft.Json;
using Ocelot.Configuration.File;
using Yarp.ReverseProxy.Configuration;

namespace SwizlyPeasy.Utils.Ocelot;

public static class OcelotConverter
{
    /// <summary>
    /// Helper method to convert an ocelot configuration file to a swizly configuration file
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string? ConvertOcelotConfigurationFile(string input)
    {
        var config = JsonConvert.DeserializeObject<FileConfiguration>(input);

        if (config?.Routes == null) return null;

        var ocelotRoutes = config.Routes;

        Dictionary<string, RouteConfig> routeConfigsDic = new();

        var id = 1;
        foreach (var route in ocelotRoutes.Where(x => x.ServiceName != null))
        {
            var routeId = $"route{id}";
            RouteConfig newConfig;

            if (route.AuthenticationOptions is { AuthenticationProviderKey: not null })
                newConfig = new RouteConfig
                {
                    ClusterId = route.ServiceName,
                    AuthorizationPolicy = "oidc",
                    Match = new RouteMatch
                    {
                        Path = route.DownstreamPathTemplate,
                        Methods = route.UpstreamHttpMethod.Select(x => x.ToUpperInvariant()).ToArray()
                    }
                };
            else
                newConfig = new RouteConfig
                {
                    ClusterId = route.ServiceName,
                    RateLimiterPolicy = "swizly1",
                    Match = new RouteMatch
                    {
                        Path = route.DownstreamPathTemplate,
                        Methods = route.UpstreamHttpMethod.Select(x => x.ToUpperInvariant()).ToArray()
                    }
                };

            routeConfigsDic.Add(routeId, newConfig);
            id++;
        }

        var baseDic = new Dictionary<string, object> { { "Routes", routeConfigsDic } };

        var routeConfigString = JsonConvert.SerializeObject(baseDic, Formatting.Indented,
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

        return routeConfigString;
    }
}