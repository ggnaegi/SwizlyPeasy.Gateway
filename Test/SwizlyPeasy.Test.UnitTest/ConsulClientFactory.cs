using System.Net;
using Consul;
using Moq;
using Newtonsoft.Json.Linq;

namespace SwizlyPeasy.Test.UnitTest;

internal class ConsulClientFactory
{
    internal static (IConsulClient Client, Mock<IKVEndpoint> KeyValueEndpoint) GetKvConsulClient()
    {
        var clientMock = new Mock<IConsulClient>();
        var keyValueMock = new Mock<IKVEndpoint>();

        keyValueMock.Setup(x => x.Put(It.IsAny<KVPair>(), It.IsAny<CancellationToken>())).ReturnsAsync(
            new WriteResult<bool>
            {
                RequestTime = new TimeSpan(0, 0, 0, 1),
                Response = true,
                StatusCode = HttpStatusCode.OK
            });

        keyValueMock.Setup(x =>
                x.Get(It.Is<string>(s => s.Equals(TestFactories.RouteConfigKey)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueryResult<KVPair>
            {
                StatusCode = HttpStatusCode.OK,
                Response = TestFactories.GetKvPair()
            });

        keyValueMock.Setup(x =>
                x.Get(It.Is<string>(s => !s.Equals(TestFactories.RouteConfigKey)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueryResult<KVPair>
            {
                StatusCode = HttpStatusCode.NotFound
            });


        clientMock.SetupGet(x => x.KV).Returns(keyValueMock.Object);
        return (clientMock.Object, keyValueMock);
    }

    internal static IConsulClient GetAgentsConsulClient(Dictionary<string, AgentService> agentsDic)
    {
        var clientMock = new Mock<IConsulClient>();
        var agentMock = new Mock<IAgentEndpoint>();

        agentMock.Setup(x => x.Services(It.IsAny<CancellationToken>())).ReturnsAsync(
            new QueryResult<Dictionary<string, AgentService>>
            {
                StatusCode = HttpStatusCode.OK,
                Response = agentsDic
            });

        clientMock.SetupGet(x => x.Agent).Returns(agentMock.Object);
        return clientMock.Object;
    }

    internal static IConsulClient GetRawConsulClient()
    {
        var clientMock = new Mock<IConsulClient>();
        clientMock.Setup(x => x.Raw.Query(It.Is<string>(s => s.Equals(TestFactories.ServiceUrl)),
                It.IsAny<QueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueryResult<dynamic>
            {
                StatusCode = HttpStatusCode.OK,
                Response = JObject.Parse(TestFactories.HealthCheckOkResult)
            });

        clientMock.Setup(x => x.Raw.Query(It.Is<string>(s => s.Equals(TestFactories.WrongUrl)),
                It.IsAny<QueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueryResult<dynamic>
            {
                StatusCode = HttpStatusCode.NotFound,
                Response = JObject.Parse(TestFactories.HealthCheckNotFoundResult)
            });

        clientMock.Setup(x => x.Raw.Query(It.Is<string>(s => s.Equals(TestFactories.EmptyUrl)),
                It.IsAny<QueryOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueryResult<dynamic>
            {
                StatusCode = HttpStatusCode.OK
            });

        return clientMock.Object;
    }
}