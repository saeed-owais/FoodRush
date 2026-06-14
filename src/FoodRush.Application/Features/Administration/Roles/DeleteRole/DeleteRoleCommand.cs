using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Roles.DeleteRole;

public sealed record DeleteRoleCommand(Guid Id) : IRequest<Result>;
