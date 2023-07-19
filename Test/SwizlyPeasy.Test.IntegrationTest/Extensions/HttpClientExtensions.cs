using System.Net;
using SwizlyPeasy.Test.IntegrationTest.Auth;

namespace SwizlyPeasy.Test.IntegrationTest.Extensions;

internal static class HttpClientExtensions
{
    internal const string SwizlyPeasyHeaderPrefix = "SWIZLY-PEASY";

    internal static void SetHeaders(this HttpClient client, bool isBob = false)
    {
        var user = isBob ? new TestUser { Sub = "2", Name = "Bob", FamilyName = "Bob" } : new TestUser();
        var claimsDic = user.GetClaims();
        foreach (var claimKeyValue in claimsDic)
        {
            var headerKey = $"{SwizlyPeasyHeaderPrefix}-{claimKeyValue.Key}";
            client.DefaultRequestHeaders.Add(headerKey, claimKeyValue.Value.ToString());
        }
    }

    internal static void ResetHeaders(this HttpClient client)
    {
        var claimsDic = new TestUser().GetClaims();
        foreach (var headerKey in claimsDic.Select(claimKeyValue => $"{SwizlyPeasyHeaderPrefix}-{claimKeyValue.Key}"))
            client.DefaultRequestHeaders.Remove(headerKey);
    }

    internal static void SetupBearerToken(this HttpClient client, bool isBob = false)
    {
        var currentUser = new TestUser();

        if (isBob)
        {
            currentUser.Sub = "2";
            currentUser.Name = "Bob";
            currentUser.FamilyName = "Bob";
        }

        client.SetFakeBearerToken(currentUser.GetClaims());
    }

    internal static void ResetBearerToken(this HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = null;
    }
}