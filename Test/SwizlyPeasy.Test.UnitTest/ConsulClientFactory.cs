using System.Net;
using Consul;
using Moq;

namespace SwizlyPeasy.Test.UnitTest;

internal class ConsulClientFactory
{
    internal static IConsulClient GetKvConsulClient()
    {
        var clientMock = new Mock<IConsulClient>();

        clientMock.Setup(x => x.KV.Put(It.IsAny<KVPair>(), It.IsAny<CancellationToken>())).ReturnsAsync(
            new WriteResult<bool>
            {
                RequestTime = new TimeSpan(0, 0, 0, 1),
                Response = true,
                StatusCode = HttpStatusCode.OK
            });

        clientMock.Setup(x =>
                x.KV.Get(It.Is<string>(s => s.Equals(TestFactories.RouteConfigKey)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueryResult<KVPair>
            {
                StatusCode = HttpStatusCode.OK,
                Response = TestFactories.GetKvPair()
            });

        clientMock.Setup(x =>
                x.KV.Get(It.Is<string>(s => !s.Equals(TestFactories.RouteConfigKey)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueryResult<KVPair>
            {
                StatusCode = HttpStatusCode.NotFound
            });


        return clientMock.Object;
    }

    internal static IConsulClient GetAgentsConsulClient(Dictionary<string, AgentService> agentsDic)
    {
        var clientMock = new Mock<IConsulClient>();

        clientMock.Setup(x => x.Agent.Services(It.IsAny<CancellationToken>())).ReturnsAsync(
            new QueryResult<Dictionary<string, AgentService>>
            {
                StatusCode = HttpStatusCode.OK,
                Response = agentsDic
            });

        return clientMock.Object;
    }
}