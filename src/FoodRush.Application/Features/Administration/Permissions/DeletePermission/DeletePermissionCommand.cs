using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Permissions.DeletePermission;

public sealed record DeletePermissionCommand(Guid Id) : IRequest<Result>;