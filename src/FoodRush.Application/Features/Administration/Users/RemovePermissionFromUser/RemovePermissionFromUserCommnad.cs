using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Users.RemovePermissionFromUser;

public sealed record RemovePermissionFromUserCommand(Guid UserId, Guid PermissionId) : IRequest<Result>;