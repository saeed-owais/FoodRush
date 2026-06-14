using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Roles.UpdateRole;

public sealed record UpdateRoleCommand(Guid Id, string Name, string Code) :
    IRequest<Result<UpdateRoleResponse>>;
