using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using SwizlyPeasy.Gateway.API;
using SwizlyPeasy.Test.IntegrationTest.Factories;

namespace SwizlyPeasy.Test.IntegrationTest;

/// <summary>
///     Http client fixture used in several test classes
///     avoiding creating a new test client every time.
/// </summary>
public class TestHttpClient<TProgram, TProgram2> : IDisposable
    where TProgram : class
    where TProgram2 : class
{
    // To detect redundant calls
    private bool _disposedValue;

    public TestHttpClient()
    {
        var factory = new CustomWebApplicationFactory<TProgram>();

        Client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var clientFactory = new CustomClientWebApplicationFactory<TProgram2>();

        DemoClient = clientFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        Client.DefaultRequestHeaders.Accept.Clear();
        Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        DemoClient.DefaultRequestHeaders.Accept.Clear();
        DemoClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public HttpClient Client { get; }

    public HttpClient DemoClient { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing)
        {
            Client.Dispose();
            DemoClient.Dispose();
        }

        _disposedValue = true;
    }
}

[CollectionDefinition("TestHttpClient")]
public class TestHttpClientCollection : ICollectionFixture<TestHttpClient<Program, Demo.API.Program>>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}