using System.Net;
using Newtonsoft.Json;
using SwizlyPeasy.Demo.API;
using SwizlyPeasy.Demo.API.Dtos;
using SwizlyPeasy.Test.IntegrationTest.Extensions;

namespace SwizlyPeasy.Test.IntegrationTest;

[Collection("TestHttpClient")]
public class ClientMicroServiceTest(TestHttpClient<Program, Program> httpClient)
    : IClassFixture<TestHttpClient<Program, Program>>
{
    private readonly TestHttpClient<Program, Program> _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    [Fact]
    public async Task ClientService_NotAuthenticatedOnAnonymousEndpoint_ReturnsOk()
    {
        var response = await _httpClient.DemoClient.GetAsync("/api/v1/demo/weather-anonymous");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var weatherForecasts = JsonConvert.DeserializeObject<WeatherForecast[]>(result);

        Assert.NotNull(weatherForecasts);
        Assert.NotEmpty(weatherForecasts);
    }

    [Fact]
    public async Task ClientService_NotAuthenticatedNotAsBob_ReturnsUnauthorized()
    {
        var response = await _httpClient.DemoClient.GetAsync("/api/v1/demo/weather-with-authorization");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ClientService_AuthenticatedButNotAsBob_ReturnsForbidden()
    {
        _httpClient.DemoClient.SetHeaders();
        var response = await _httpClient.DemoClient.GetAsync("/api/v1/demo/weather-with-authorization");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        _httpClient.DemoClient.ResetHeaders();
    }

    [Fact]
    public async Task ClientService_AuthenticatedAsBob_ReturnsOk()
    {
        _httpClient.DemoClient.SetHeaders(true);
        var response = await _httpClient.DemoClient.GetAsync("/api/v1/demo/weather-with-authorization");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var weatherForecasts = JsonConvert.DeserializeObject<WeatherForecast[]>(result);

        Assert.NotNull(weatherForecasts);
        Assert.NotEmpty(weatherForecasts);

        _httpClient.DemoClient.ResetHeaders();
    }
}