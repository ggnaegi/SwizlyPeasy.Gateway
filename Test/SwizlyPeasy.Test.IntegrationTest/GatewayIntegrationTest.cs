using System.Net;
using Newtonsoft.Json;
using SwizlyPeasy.Common.Dtos;
using SwizlyPeasy.Common.Dtos.Status;
using SwizlyPeasy.Demo.API.Dtos;
using SwizlyPeasy.Gateway.API;

namespace SwizlyPeasy.Test.IntegrationTest;

[Collection("TestHttpClient")]
public class GatewayIntegrationTest(TestHttpClient<Program, Demo.API.Program> httpClient)
    : IClassFixture<TestHttpClient<Program, Demo.API.Program>>
{
    private readonly TestHttpClient<Program, Demo.API.Program> _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    [Fact]
    public async Task Gateway_GetRoot_OkAndReturnsClusters()
    {
        var response = await _httpClient.Client.GetAsync("");

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        var serverStatus = JsonConvert.DeserializeObject<StatusDto>(result);

        Assert.NotNull(serverStatus);

        var storedStatus = JsonConvert.DeserializeObject<StatusDto>(Constants.GatewayRootResponse);

        Assert.NotNull(storedStatus);

        var timeNow = DateTime.Now;
        serverStatus.StatusCheckedAt = timeNow;
        storedStatus.StatusCheckedAt = timeNow;

        Assert.Equal(storedStatus.ServiceDiscoveryAddress, serverStatus.ServiceDiscoveryAddress);

        Assert.Equal(JsonConvert.SerializeObject(storedStatus), JsonConvert.SerializeObject(serverStatus));
    }

    [Fact]
    public async Task Gateway_GetUnknownPath_NotFound()
    {
        var response = await _httpClient.Client.GetAsync("/test");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();

        var rfcNotFound = JsonConvert.DeserializeObject<Rfc7807>(result);

        Assert.NotNull(rfcNotFound);

        Assert.Equal(404, rfcNotFound.Status);
    }

    [Fact]
    public async Task Gateway_GetAnonymousWeatherPath_Ok()
    {
        var response = await _httpClient.Client.GetAsync("/api/v1/demo/weather-anonymous");

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        Assert.NotNull(result);

        var weatherObjects = JsonConvert.DeserializeObject<WeatherForecast[]>(result);

        Assert.NotNull(weatherObjects);
    }
}