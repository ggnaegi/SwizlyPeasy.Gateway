using Newtonsoft.Json;
using SwizlyPeasy.Common.HealthChecks;
using SwizlyPeasy.Demo.API;

namespace SwizlyPeasy.Test.IntegrationTest;

[Collection("TestHttpClient")]
public class HealthCheckTest(TestHttpClient<Program, Demo.API.Program> httpClient)
    : IClassFixture<TestHttpClient<Program, Demo.API.Program>>
{
    private readonly TestHttpClient<Program, Demo.API.Program> _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    [Fact]
    public async Task ClientService_GetHealthCheck_ReturnsOk()
    {
        var response = await _httpClient.DemoClient.GetAsync("/health");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        Assert.NotNull(result);

        var healthCheck = JsonConvert.DeserializeObject<HealthCheckResponse>(result);

        Assert.NotNull(healthCheck);
        Assert.Equal("Healthy", healthCheck.Status);
    }
}