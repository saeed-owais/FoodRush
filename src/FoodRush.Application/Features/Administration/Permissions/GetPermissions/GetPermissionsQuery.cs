using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Permissions.GetPermissions;

public sealed record GetPermissionsQuery : IRequest<Result<List<PermissionResponse>>>;
