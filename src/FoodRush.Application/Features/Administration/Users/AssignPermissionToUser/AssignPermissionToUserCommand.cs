using FoodRush.Domain.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Users.AssignPermissionToUser;

public sealed record AssignPermissionToUserCommand(Guid UserId, Guid PermissionId)
    : IRequest<Result>;
