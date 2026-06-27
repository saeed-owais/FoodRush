using FoodRush.Domain.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Roles.CreateRole;

public sealed record CreateRoleCommand(string Name, string Code) : IRequest<Result<CreateRoleResponse>>;