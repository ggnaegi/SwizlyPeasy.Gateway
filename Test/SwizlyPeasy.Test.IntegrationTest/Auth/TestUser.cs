using IdentityModel;

namespace SwizlyPeasy.Test.IntegrationTest.Auth;

internal class TestUser
{
    internal string Sub { get; set; } = "1";
    internal string Email { get; set; } = "henri.du.test@test.com";
    internal string Name { get; set; } = "Henri";
    internal string FamilyName { get; set; } = "Du Test";

    internal Dictionary<string, object> GetClaims()
    {
        return new Dictionary<string, object>
        {
            { JwtClaimTypes.Subject, Sub },
            { JwtClaimTypes.Name, Name }, 
            { JwtClaimTypes.Email, Email },
            { JwtClaimTypes.FamilyName, FamilyName }
        };
    }
}