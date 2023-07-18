using System.Net;
using Newtonsoft.Json;
using SwizlyPeasy.Demo.API.Dtos;
using SwizlyPeasy.Gateway.API;
using SwizlyPeasy.Test.IntegrationTest.Auth;

namespace SwizlyPeasy.Test.IntegrationTest;

public class AuthIntegrationTest : IClassFixture<TestHttpClient<Program>>
{
    private readonly TestHttpClient<Program> _httpClient;

    public AuthIntegrationTest(TestHttpClient<Program> httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }


    [Fact]
    public async Task Auth_GetRouteNotAuthenticated_ReturnsUnauthorized()
    {
        var response = await _httpClient.Client.GetAsync("api/v1/demo/weather");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Auth_GetRouteAuthenticated_ReturnsOk()
    {
        SetupBearerToken();
        var response = await _httpClient.Client.GetAsync("api/v1/demo/weather");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var weatherForecasts = JsonConvert.DeserializeObject<WeatherForecast[]>(content);

        Assert.NotNull(weatherForecasts);
        Assert.NotEmpty(weatherForecasts);

        ResetBearerToken();
    }

    [Fact]
    public async Task Auth_GetRouteAuthenticatedButNotBob_ReturnsForbidden()
    {
        SetupBearerToken();
        var response = await _httpClient.Client.GetAsync("api/v1/demo/weather-with-authorization");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        ResetBearerToken();
    }

    [Fact]
    public async Task Auth_GetRouteAuthenticatedBob_ReturnsOk()
    {
        SetupBearerToken(true);
        var response = await _httpClient.Client.GetAsync("api/v1/demo/weather-with-authorization");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var weatherForecasts = JsonConvert.DeserializeObject<WeatherForecast[]>(content);

        Assert.NotNull(weatherForecasts);
        Assert.NotEmpty(weatherForecasts);

        ResetBearerToken();
    }

    private void SetupBearerToken(bool isBob = false)
    {
        var currentUser = new TestUser();

        if (isBob)
        {
            currentUser.Sub = "2";
            currentUser.Name = "Bob";
            currentUser.FamilyName = "Bob";
        }

        _httpClient.Client.SetFakeBearerToken(currentUser.GetClaims());
    }

    private void ResetBearerToken()
    {
        _httpClient.Client.DefaultRequestHeaders.Authorization = null;
    }
}