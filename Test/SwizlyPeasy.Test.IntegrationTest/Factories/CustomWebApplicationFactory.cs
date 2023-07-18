#region

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using WebMotions.Fake.Authentication.JwtBearer;

#endregion

namespace SwizlyPeasy.Test.IntegrationTest.Factories;

/// <summary>
///     Factory that can be used to to create a TestServer instance
/// </summary>
/// <typeparam name="TProgram">Type of the entry point assembly</typeparam>
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    /// <summary>
    /// </summary>
    /// <param name="webHostBuilder"></param>
    /// <exception cref="Exception"></exception>
    protected override void ConfigureWebHost(IWebHostBuilder webHostBuilder)
    {
        webHostBuilder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(AuthenticationBuilder));

            if(descriptor != null)
                services.Remove(descriptor);

            services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme).AddFakeJwtBearer();
        });

        webHostBuilder.UseEnvironment("IntegrationTest");
    }
}