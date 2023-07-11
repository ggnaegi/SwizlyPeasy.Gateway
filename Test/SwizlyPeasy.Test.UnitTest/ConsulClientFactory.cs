using System.Net;
using Consul;
using Moq;
using Newtonsoft.Json.Linq;

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