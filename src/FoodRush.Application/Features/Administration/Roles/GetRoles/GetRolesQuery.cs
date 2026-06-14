using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Roles.GetRoles;

public sealed record GetRolesQuery : IRequest<Result<List<RoleResponse>>>;