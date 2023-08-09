using System.Text;
using Microsoft.Extensions.Configuration;
using SwizlyPeasy.Common.Exceptions;
using SwizlyPeasy.Consul.KeyValueStore;
using SwizlyPeasy.Gateway.Extensions;
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
        var configuration = new ConfigurationBuilder().AddJsonStream(new MemoryStream(rawBytes)).Build();

        return configuration.GetSection("Routes").GetChildren().Select(CreateRoute).ToArray();
    }

    private static RouteConfig CreateRoute(IConfigurationSection section)
    {
        if (!string.IsNullOrEmpty(section["RouteId"]))
            throw new InternalDomainException(
                "The route config format has changed, routes are now objects instead of an array. The route id must be set as the object name, not with the 'RouteId' field.", null);

        return new RouteConfig
        {
            RouteId = section.Key,
            Order = section.ReadInt32(nameof(RouteConfig.Order)),
            MaxRequestBodySize = section.ReadInt64(nameof(RouteConfig.MaxRequestBodySize)),
            ClusterId = section[nameof(RouteConfig.ClusterId)],
            AuthorizationPolicy = section[nameof(RouteConfig.AuthorizationPolicy)],
            RateLimiterPolicy = section[nameof(RouteConfig.RateLimiterPolicy)],
            CorsPolicy = section[nameof(RouteConfig.CorsPolicy)],
            Metadata = section.GetSection(nameof(RouteConfig.Metadata)).ReadStringDictionary(),
            Transforms = CreateTransforms(section.GetSection(nameof(RouteConfig.Transforms))),
            Match = CreateRouteMatch(section.GetSection(nameof(RouteConfig.Match)))
        };
    }

    private static IReadOnlyList<IReadOnlyDictionary<string, string>>? CreateTransforms(IConfigurationSection section)
    {
        if (section.GetChildren() is var children && !children.Any()) return null;

        return children.Select(subSection =>
                subSection.GetChildren().ToDictionary(d => d.Key, d => d.Value!, StringComparer.OrdinalIgnoreCase))
            .ToList();
    }

    private static RouteMatch CreateRouteMatch(IConfigurationSection section)
    {
        if (!section.Exists()) return new RouteMatch();

        return new RouteMatch
        {
            Methods = section.GetSection(nameof(RouteMatch.Methods)).ReadStringArray(),
            Hosts = section.GetSection(nameof(RouteMatch.Hosts)).ReadStringArray(),
            Path = section[nameof(RouteMatch.Path)],
            Headers = CreateRouteHeaders(section.GetSection(nameof(RouteMatch.Headers))),
            QueryParameters = CreateRouteQueryParameters(section.GetSection(nameof(RouteMatch.QueryParameters)))
        };
    }

    private static IReadOnlyList<RouteHeader>? CreateRouteHeaders(IConfigurationSection section)
    {
        return !section.Exists() ? null : section.GetChildren().Select(CreateRouteHeader).ToArray();
    }

    private static RouteHeader CreateRouteHeader(IConfigurationSection section)
    {
        return new RouteHeader
        {
            Name = section[nameof(RouteHeader.Name)]!,
            Values = section.GetSection(nameof(RouteHeader.Values)).ReadStringArray(),
            Mode = section.ReadEnum<HeaderMatchMode>(nameof(RouteHeader.Mode)) ?? HeaderMatchMode.ExactHeader,
            IsCaseSensitive = section.ReadBool(nameof(RouteHeader.IsCaseSensitive)) ?? false
        };
    }

    private static IReadOnlyList<RouteQueryParameter>? CreateRouteQueryParameters(IConfigurationSection section)
    {
        return !section.Exists() ? null : section.GetChildren().Select(CreateRouteQueryParameter).ToArray();
    }

    private static RouteQueryParameter CreateRouteQueryParameter(IConfigurationSection section)
    {
        return new RouteQueryParameter
        {
            Name = section[nameof(RouteQueryParameter.Name)]!,
            Values = section.GetSection(nameof(RouteQueryParameter.Values)).ReadStringArray(),
            Mode = section.ReadEnum<QueryParameterMatchMode>(nameof(RouteQueryParameter.Mode)) ??
                   QueryParameterMatchMode.Exact,
            IsCaseSensitive = section.ReadBool(nameof(RouteQueryParameter.IsCaseSensitive)) ?? false
        };
    }
}