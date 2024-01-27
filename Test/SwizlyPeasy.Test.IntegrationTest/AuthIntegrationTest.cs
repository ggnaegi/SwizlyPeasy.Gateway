using System.Net;
using Newtonsoft.Json;
using SwizlyPeasy.Demo.API.Dtos;
using SwizlyPeasy.Gateway.API;
using SwizlyPeasy.Test.IntegrationTest.Extensions;

namespace SwizlyPeasy.Test.IntegrationTest;

[Collection("TestHttpClient")]
public class AuthIntegrationTest(TestHttpClient<Program, Demo.API.Program> httpClient)
    : IClassFixture<TestHttpClient<Program, Demo.API.Program>>
{
    private readonly TestHttpClient<Program, Demo.API.Program> _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));


    [Fact]
    public async Task Auth_GetRouteNotAuthenticated_ReturnsUnauthorized()
    {
        var response = await _httpClient.Client.GetAsync("api/v1/demo/weather");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Auth_GetRouteAuthenticated_ReturnsOk()
    {
        _httpClient.Client.SetupBearerToken();
        var response = await _httpClient.Client.GetAsync("api/v1/demo/weather");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var weatherForecasts = JsonConvert.DeserializeObject<WeatherForecast[]>(content);

        Assert.NotNull(weatherForecasts);
        Assert.NotEmpty(weatherForecasts);

        _httpClient.Client.ResetBearerToken();
    }

    [Fact]
    public async Task Auth_GetRouteAuthenticatedButNotBob_ReturnsForbidden()
    {
        _httpClient.Client.SetupBearerToken();
        var response = await _httpClient.Client.GetAsync("api/v1/demo/weather-with-authorization");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        _httpClient.Client.ResetBearerToken();
    }

    [Fact]
    public async Task Auth_GetRouteAuthenticatedBob_ReturnsOk()
    {
        _httpClient.Client.SetupBearerToken(true);
        var response = await _httpClient.Client.GetAsync("api/v1/demo/weather-with-authorization");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var weatherForecasts = JsonConvert.DeserializeObject<WeatherForecast[]>(content);

        Assert.NotNull(weatherForecasts);
        Assert.NotEmpty(weatherForecasts);

        _httpClient.Client.ResetBearerToken();
    }

    [Fact]
    public async Task OidcAuth_GetAjaxRouteNotAuthenticated_ReturnsUnauthorized()
    {
        var response = await _httpClient.AjaxClient.GetAsync("api/v1/demo/weather");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}