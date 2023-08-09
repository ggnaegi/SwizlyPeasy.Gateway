using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwizlyPeasy.Common.Exceptions;
using SwizlyPeasy.Consul.KeyValueStore;
using Yarp.ReverseProxy.Configuration;

namespace SwizlyPeasy.Gateway.Services;

public class RoutesConfigService : IRoutesConfigService
{
    private readonly IKeyValueService _keyValueService;

    public RoutesConfigService(IKeyValueService keyValueService)
    {
        _keyValueService = keyValueService ?? throw new ArgumentNullException(nameof(keyValueService));
    }

    public async Task LoadRoutes(string key, string sectionString)
    {
        await _keyValueService.SaveToKeyValueStore(key, Encoding.UTF8.GetBytes(sectionString));
    }

    public async Task<RouteConfig[]> GetRoutes(string key)
    {
        var rawBytes = await _keyValueService.GetFromKeyValueStore(key);
        var configString = Encoding.UTF8.GetString(rawBytes);

        var configDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(configString);

        if (configDic == null || !configDic.ContainsKey("Routes"))
            throw new InternalDomainException("Please check the routes.config.json file, Routes key couldn't be found.",
                null);

        var routesDic = JObject.FromObject(configDic["Routes"]).ToObject<Dictionary<string, RouteConfig>>();

        return routesDic == null
            ? throw new InternalDomainException("Please check the routes.config.json files, no routes defined.", null)
            : (from routeId in routesDic.Keys
                let routeConfig = routesDic[routeId]
                select routeConfig with { RouteId = routeId }).ToArray();
    }
}