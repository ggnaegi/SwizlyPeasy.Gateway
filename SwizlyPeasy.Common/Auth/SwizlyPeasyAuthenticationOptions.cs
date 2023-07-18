using Microsoft.AspNetCore.Authentication;

namespace SwizlyPeasy.Common.Auth;

public class SwizlyPeasyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "SwizlyPeasyAuthScheme";
}