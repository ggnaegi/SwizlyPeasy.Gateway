using System.Security.Claims;
using Newtonsoft.Json;
using SwizlyPeasy.Demo.API;
using SwizlyPeasy.Test.IntegrationTest.Auth;
using SwizlyPeasy.Test.IntegrationTest.Extensions;

namespace SwizlyPeasy.Test.IntegrationTest;

[Collection("TestHttpClient")]
public class ClaimsIntegrationTest(TestHttpClient<Program, Program> httpClient)
    : IClassFixture<TestHttpClient<Program, Program>>
{
    private readonly TestHttpClient<Program, Program> _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    [Fact]
    public async Task ClientService_AuthenticatedGetClaims_ReturnsOkAndClaimsMatchConfig()
    {
        _httpClient.DemoClient.SetHeaders();
        var response = await _httpClient.DemoClient.GetAsync("/api/v1/demo/user-claims");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        Assert.NotNull(result);

        var claimsDic = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(result);

        Assert.NotNull(claimsDic);
        Assert.NotEmpty(claimsDic);

        Assert.Equal(6, claimsDic.Keys.Count);

        var testUser = new TestUser();
        var testUserDic = testUser.GetClaims();

        foreach (var key in testUserDic.Keys)
        {
            Assert.Contains(key, claimsDic.Keys);
            var value = claimsDic[key].FirstOrDefault();
            Assert.NotNull(value);
            Assert.Equal(testUserDic[key].ToString(), value);
        }

        Assert.Contains(ClaimTypes.NameIdentifier, claimsDic.Keys);
        Assert.Contains(ClaimTypes.Email, claimsDic.Keys);

        _httpClient.DemoClient.ResetHeaders();
    }

    [Fact]
    public async Task Gateway_AuthenticatedGetClaims_ReturnsOkAndClaimsMatchConfig()
    {
        _httpClient.Client.SetupBearerToken();
        var response = await _httpClient.Client.GetAsync("/api/v1/demo/user-claims");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        Assert.NotNull(result);

        var claimsDic = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(result);

        Assert.NotNull(claimsDic);
        Assert.NotEmpty(claimsDic);

        Assert.Equal(4, claimsDic.Keys.Count);

        var testUser = new TestUser();
        var testUserDic = testUser.GetClaims();

        foreach (var key in testUserDic.Keys)
        {
            Assert.Contains(key, claimsDic.Keys);
            var value = claimsDic[key].FirstOrDefault();
            Assert.NotNull(value);
            Assert.Equal(testUserDic[key].ToString(), value);
        }

        _httpClient.Client.ResetBearerToken();
    }
}