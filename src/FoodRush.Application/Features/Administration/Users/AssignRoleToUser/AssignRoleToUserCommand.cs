using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Users.AssignRoleToUser;

public sealed record AssignRoleToUserCommand(Guid UserId, Guid RoleId)
    : IRequest<Result>;