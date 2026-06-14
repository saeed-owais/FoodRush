using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Users.DeleteUser;

public sealed record DeleteUserCommand(Guid UserId) : IRequest<Result>;