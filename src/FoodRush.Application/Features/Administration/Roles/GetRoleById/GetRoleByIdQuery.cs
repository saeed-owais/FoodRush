using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Roles.GetRoleById;

public sealed record GetRoleByIdQuery(
    Guid RoleId) : IRequest<Result<RoleResponse>>;
