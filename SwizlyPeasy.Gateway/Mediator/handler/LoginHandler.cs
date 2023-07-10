using System.Security.Claims;
using MediatR;
using SwizlyPeasy.Common.Dtos;
using SwizlyPeasy.Common.Exceptions;

namespace SwizlyPeasy.Gateway.Mediator.handler
{
    public class LoginHandler : IRequestHandler<LoginRequest, UserDto>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<LoginHandler> _logger;

        public LoginHandler(
            IHttpContextAccessor contextAccessor,
            ILogger<LoginHandler> logger)
        {
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _logger = logger;
        }

        public Task<UserDto> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            //http context can't be null
            if (_contextAccessor.HttpContext == null)
            {
                throw new InternalDomainException("Http context is null", null);
            }
            _logger.LogInformation("Handling new login from ip {RemoteIpAddress}.", _contextAccessor.HttpContext.Connection.RemoteIpAddress);

            return Task.FromResult(new UserDto
            {
                Email = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value,
                Sub = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            });
        }
    }
}