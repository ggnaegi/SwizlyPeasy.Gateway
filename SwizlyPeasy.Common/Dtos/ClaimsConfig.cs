namespace SwizlyPeasy.Common.Dtos;

public class ClaimsConfig
{
    public string ClaimsHeaderPrefix { get; set; } = "SWIZLY-PEASY";
    public string[] ClaimsAsHeaders { get; set; } = Array.Empty<string>();
    public Dictionary<string, string> JwtToIdentityClaimsMappings { get; set; } = new();
}