using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using SwizlyPeasy.Test.IntegrationTest.Factories;

namespace SwizlyPeasy.Test.IntegrationTest;

/// <summary>
///     Http client fixture used in several test classes
///     avoiding creating a new test client every time.
/// </summary>
public class TestHttpClient<TProgram> : IDisposable
    where TProgram : class
{
    public TestHttpClient()
    {
        var factory = new CustomWebApplicationFactory<TProgram>();

        Client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        Client.DefaultRequestHeaders.Accept.Clear();
        Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public HttpClient Client { get; }

    public void Dispose()
    {
        Client.Dispose();
    }
}

[CollectionDefinition("TestHttpClient")]
public class TestHttpClientCollection : ICollectionFixture<TestHttpClient<Gateway.API.Program>>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}