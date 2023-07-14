using System.Security.Claims;
using IdentityModel;

namespace SwizlyPeasy.Common.Dtos;

public class ClaimsConfig
{
    public string ClaimsHeaderPrefix { get; set; } = "SWIZLY-PEASY";
    public string[] ClaimsAsHeaders { get; set; } =
    {
        JwtClaimTypes.Subject,
        JwtClaimTypes.Email,
        JwtClaimTypes.Name,
        JwtClaimTypes.FamilyName
    };

    public Dictionary<string, string> JwtToIdentityClaimsMappings { get; set; } = new()
    {
        { JwtClaimTypes.Subject, ClaimTypes.NameIdentifier },
        { JwtClaimTypes.Email, ClaimTypes.Email }
    };
}