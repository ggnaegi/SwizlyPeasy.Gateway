using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using SwizlyPeasy.Demo.API;
using SwizlyPeasy.Demo.API.Dtos;
using SwizlyPeasy.Test.IntegrationTest.Auth;

namespace SwizlyPeasy.Test.IntegrationTest;

[Collection("TestHttpClient")]
public class ClientMicroServiceTest : IClassFixture<TestHttpClient<Program, Program>>
{
    private readonly TestHttpClient<Program, Program> _httpClient;

    public ClientMicroServiceTest(TestHttpClient<Program, Program> httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

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
        SetHeaders();
        var response = await _httpClient.DemoClient.GetAsync("/api/v1/demo/weather-with-authorization");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        ResetHeaders();
    }

    [Fact]
    public async Task ClientService_AuthenticatedAsBob_ReturnsOk()
    {
        SetHeaders(true);
        var response = await _httpClient.DemoClient.GetAsync("/api/v1/demo/weather-with-authorization");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();
        var weatherForecasts = JsonConvert.DeserializeObject<WeatherForecast[]>(result);

        Assert.NotNull(weatherForecasts);
        Assert.NotEmpty(weatherForecasts);

        ResetHeaders();
    }


    private void SetHeaders(bool isBob = false)
    {
        var user = isBob ? new TestUser { Sub = "2", Name = "Bob", FamilyName = "Bob" } : new TestUser();
        var claimsDic = user.GetClaims();
        foreach (var claimKeyValue in claimsDic)
        {
            var headerKey = $"SWIZLY-PEASY-{claimKeyValue.Key}";
            _httpClient.DemoClient.DefaultRequestHeaders.Add(headerKey, claimKeyValue.Value.ToString());
        }
    }

    private void ResetHeaders()
    {
        _httpClient.DemoClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "your_token");
        var claimsDic = new TestUser().GetClaims();
        foreach (var headerKey in claimsDic.Select(claimKeyValue => $"SWIZLY-PEASY-{claimKeyValue.Key}"))
            _httpClient.DemoClient.DefaultRequestHeaders.Remove(headerKey);
    }
}