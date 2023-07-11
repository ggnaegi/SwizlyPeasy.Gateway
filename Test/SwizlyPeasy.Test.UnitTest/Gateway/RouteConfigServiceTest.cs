using Consul;
using SwizlyPeasy.Common.Dtos;
using SwizlyPeasy.Gateway.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Test.UnitTest.Gateway
{
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
}
