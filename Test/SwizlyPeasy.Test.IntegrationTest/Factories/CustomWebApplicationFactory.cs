#region

using System.IdentityModel.Tokens.Jwt;
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
public class CustomWebApplicationFactory<TProgram>(bool useOidc = false) : WebApplicationFactory<TProgram>
    where TProgram : class
{
    /// <summary>
    /// </summary>
    /// <param name="webHostBuilder"></param>
    /// <exception cref="Exception"></exception>
    protected override void ConfigureWebHost(IWebHostBuilder webHostBuilder)
    {
        if (useOidc)
        {
            webHostBuilder.UseEnvironment("IntegrationTest2");
            return;
        }

        webHostBuilder.ConfigureServices(services =>
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme).AddFakeJwtBearer();
        });

        webHostBuilder.UseEnvironment("IntegrationTest");
    }
}

/// <summary>
///     Factory that can be used to to create a TestServer instance
/// </summary>
/// <typeparam name="TProgram">Type of the entry point assembly</typeparam>
public class CustomClientWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    /// <summary>
    /// </summary>
    /// <param name="webHostBuilder"></param>
    /// <exception cref="Exception"></exception>
    protected override void ConfigureWebHost(IWebHostBuilder webHostBuilder)
    {
        webHostBuilder.UseEnvironment("IntegrationTest3");
    }
}