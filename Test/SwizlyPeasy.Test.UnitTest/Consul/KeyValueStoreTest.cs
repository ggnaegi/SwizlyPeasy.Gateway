using System.Text;
using Consul;
using Moq;
using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Test.UnitTest.Consul;

public class KeyValueStoreTest
{
    [Fact]
    public async Task SaveToKeyValueStore_DoesNotThrowException()
    {
        var keyValueService = TestFactories.GetKeyValueService(out var keyValueEndpoint);
        var value = Encoding.UTF8.GetBytes(TestFactories.RouteConfigString);

        await keyValueService.SaveToKeyValueStore(TestFactories.RouteConfigKey, value);

        keyValueEndpoint.Verify(
            endpoint => endpoint.Put(
                It.Is<KVPair>(pair => pair.Key == TestFactories.RouteConfigKey &&
                                      pair.Value.SequenceEqual(value)),
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public async Task GetToKeyValueStore_UsingExistingKey_ReturnsResult()
    {
        var keyValueService = TestFactories.GetKeyValueService();
        var valueBytes = await keyValueService.GetFromKeyValueStore(TestFactories.RouteConfigKey);

        var stringValue = Encoding.UTF8.GetString(valueBytes);
        Assert.Equal(TestFactories.RouteConfigString, stringValue);
    }

    [Fact]
    public async Task GetToKeyValueStore_UsingNotExistingKey_ThrowsException()
    {
        var keyValueService = TestFactories.GetKeyValueService();
        await Assert.ThrowsAsync<InternalDomainException>(
            async () => await keyValueService.GetFromKeyValueStore("test"));
    }
}