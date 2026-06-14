using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Users.GetUserRoles;

public sealed record GetUserRolesQuery(Guid UserId)
    : IRequest<Result<IReadOnlyCollection<GetUserRoleResponse>>>;