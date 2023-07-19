using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwizlyPeasy.Demo.API.Dtos;

namespace SwizlyPeasy.Demo.API.Controllers;

[ApiController]
[Route("api/v1/demo")]
public class DemoController : ControllerBase
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet("weather")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }

    [HttpGet("weather-anonymous")]
    public IEnumerable<WeatherForecast> GetAnonymous()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }

    [Authorize("AreYouBob")]
    [HttpGet("weather-with-authorization")]
    public IEnumerable<WeatherForecast> GetWithAuthorization()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }

    [Authorize]
    [HttpGet("user-claims")]
    public Dictionary<string, List<string>> GetClaims()
    {
        var claimsDictionary = new Dictionary<string, List<string>>();
        var userClaims = HttpContext.User.Claims;
        foreach (var userClaim in userClaims)
        {
            if (!claimsDictionary.ContainsKey(userClaim.Type))
            {
                claimsDictionary.Add(userClaim.Type, new List<string>());
            }

            claimsDictionary[userClaim.Type].Add(userClaim.Value);
        }
        return claimsDictionary;
    }
}