using System.Net;
using System.Text;
using Consul;
using Newtonsoft.Json;
using SwizlyPeasy.Consul.KeyValueStore;
using SwizlyPeasy.Demo.API.Dtos;
using SwizlyPeasy.Gateway.API;

namespace SwizlyPeasy.Test.IntegrationTest;

[Collection("TestHttpClient")]
public class DynamicConfigIntegrationTest(TestHttpClient<Program, Demo.API.Program> httpClient)
    : IClassFixture<TestHttpClient<Program, Demo.API.Program>>
{
    private readonly TestHttpClient<Program, Demo.API.Program> _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    [Fact]
    public async Task Config_ModifyingConfigInKvStore_AfterElapsedTimeNewConfigLoadedAndEndpointReachable()
    {
        var consulClient = new ConsulClient(config => { config.Address = new Uri("http://localhost:8500"); });
        var kvService = new KeyValueService(consulClient);

        var response = await _httpClient.Client.GetAsync("/newpath");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        await kvService.SaveToKeyValueStore("SwizlyPeasy.Gateway",
            Encoding.UTF8.GetBytes(Constants.GatewayConfigWithPathSet));

        await Task.Delay(30000);

        response = await _httpClient.Client.GetAsync("/newpath");
        response.EnsureSuccessStatusCode();

        var resultString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(resultString);

        var weatherForecast = JsonConvert.DeserializeObject<WeatherForecast[]>(resultString);

        Assert.NotNull(weatherForecast);
        Assert.NotEmpty(weatherForecast);

        await kvService.SaveToKeyValueStore("SwizlyPeasy.Gateway", Encoding.UTF8.GetBytes(Constants.GatewayBaseConfig));

        await Task.Delay(30000);

        response = await _httpClient.Client.GetAsync("/newpath");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        response = await _httpClient.Client.GetAsync("/api/v1/demo/weather-anonymous");
        response.EnsureSuccessStatusCode();

        resultString = await response.Content.ReadAsStringAsync();

        Assert.NotNull(resultString);

        weatherForecast = JsonConvert.DeserializeObject<WeatherForecast[]>(resultString);

        Assert.NotNull(weatherForecast);
        Assert.NotEmpty(weatherForecast);
    }
}