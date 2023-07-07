using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Demo.API.Authorization
{
    public class AppAuthorizationHandler : AuthorizationHandler<BobRequirement>
    {
        private readonly IServiceProvider _serviceProvider;

        public AppAuthorizationHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BobRequirement requirement)
        {
            var user = context.User;
            if (user is not { Identity.IsAuthenticated: true }) return Task.CompletedTask;

            using var scope = _serviceProvider.CreateScope();
            var identifierClaim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (identifierClaim != null && identifierClaim.Value == requirement.GetBobSub)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            
            throw new ForbiddenDomainException("You're not Bob!");
        }
    }
}
