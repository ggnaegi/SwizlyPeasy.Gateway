﻿#region

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        webHostBuilder.UseEnvironment("IntegrationTest");

    }
}