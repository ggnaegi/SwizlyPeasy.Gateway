using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Model.Common;
using Ductus.FluentDocker.Services;

namespace SwizlyPeasy.Test.IntegrationTest;

/// <summary>
///     Http client fixture used in several test classes
///     avoiding creating a new test client every time.
/// </summary>
public class TestHttpClient : IDisposable
{
    private readonly ICompositeService _svc;

    public TestHttpClient()
    {
        Client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:8001")
        };

        var composeFile = Path.Combine(Directory.GetCurrentDirectory(),
            (TemplateString)"Docker/docker-compose.yml");

        var overrideComposeFile = Path.Combine(Directory.GetCurrentDirectory(),
            (TemplateString)"Docker/docker-compose.override.yml");

            // @formatter:off
            _svc = new Builder()
                .UseContainer()
                .UseCompose()
                .FromFile(composeFile, overrideComposeFile)
                .RemoveOrphans()
                .Build().Start();

            Thread.Sleep(10000);
    }

    public HttpClient Client { get; }

    public void Dispose()
    {
        Client.Dispose();
        _svc.Dispose();
    }
}

[CollectionDefinition("TestHttpClient")]
public class TestHttpClientCollection : ICollectionFixture<TestHttpClient>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.}
}