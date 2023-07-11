using MediatR;
using SwizlyPeasy.Common.Dtos;

namespace SwizlyPeasy.Gateway.Mediator;

public class LoginRequest : IRequest<UserDto>
{
}