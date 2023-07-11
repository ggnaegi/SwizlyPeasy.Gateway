using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Test.UnitTest.Consul;

public class HealthCheckTest
{
    [Fact]
    public async Task IsServiceHealthy_ExistingServiceId_ReturnsTrue()
    {
        var healthCheckService = TestFactories.GetHealthCheckService();
        var result = await healthCheckService.IsServiceHealthy(TestFactories.ServiceId);

        Assert.True(result);
    }

    [Fact]
    public async Task IsServiceHealthy_NotExistingServiceId_ThrowsException()
    {
        var healthCheckService = TestFactories.GetHealthCheckService();
        await Assert.ThrowsAsync<InternalDomainException>(async () =>
            await healthCheckService.IsServiceHealthy(TestFactories.WrongId));
    }

    [Fact]
    public async Task IsServiceHealthy_ExistingServiceIdButNullResponse_ThrowsException()
    {
        var healthCheckService = TestFactories.GetHealthCheckService();
        await Assert.ThrowsAsync<InternalDomainException>(async () =>
            await healthCheckService.IsServiceHealthy(TestFactories.EmptyId));
    }
}