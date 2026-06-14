using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Permissions.CreatePermission;

public sealed record CreatePermissionCommand(string Name, string Code)
    : IRequest<Result<CreatePermissionResponse>>;