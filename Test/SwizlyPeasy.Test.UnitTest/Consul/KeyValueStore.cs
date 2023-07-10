using System.Text;
using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Test.UnitTest.Consul;

public class KeyValueStore
{
    [Fact]
    public async Task SaveToKeyValueStore_DoesNotThrowException()
    {
        var keyValueService = TestFactories.GetKeyValueService();
        await keyValueService.SaveToKeyValueStore(TestFactories.RouteConfigKey,
            Encoding.UTF8.GetBytes(TestFactories.RouteConfigString));
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
        await Assert.ThrowsAsync<InternalDomainException>(async() => await keyValueService.GetFromKeyValueStore("test"));
    }
}