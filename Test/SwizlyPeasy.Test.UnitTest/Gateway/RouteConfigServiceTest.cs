using SwizlyPeasy.Common.Exceptions;
using SwizlyPeasy.Gateway.Services;

namespace SwizlyPeasy.Test.UnitTest.Gateway;

public class RouteConfigServiceTest
{
    [Fact]
    public async Task GetRouteConfig_UseWrongKey_ThrowsException()
    {
        var keyValueService = TestFactories.GetKeyValueService();
        var routeConfigService = new RoutesConfigService(keyValueService);
        await Assert.ThrowsAsync<InternalDomainException>(async () => await routeConfigService.GetRoutes("test"));
    }

    [Fact]
    public async Task GetRouteConfig_UseKnownKey_ReturnsConfigArray()
    {
        var keyValueService = TestFactories.GetKeyValueService();
        var routeConfigService = new RoutesConfigService(keyValueService);
        var result = await routeConfigService.GetRoutes(TestFactories.RouteConfigKey);

        Assert.NotNull(result);
        Assert.Equal(2, result.Length);

        foreach (var routeConfig in result)
        {
            Assert.Equal("DemoAPI", routeConfig.ClusterId);
            Assert.Equal("oidc", routeConfig.AuthorizationPolicy);
        }
    }
}