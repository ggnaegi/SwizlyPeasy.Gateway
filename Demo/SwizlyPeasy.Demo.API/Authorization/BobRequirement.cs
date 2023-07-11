using Microsoft.AspNetCore.Authorization;

namespace SwizlyPeasy.Demo.API.Authorization;

public class BobRequirement : IAuthorizationRequirement
{
    public string GetBobSub => "2";
}