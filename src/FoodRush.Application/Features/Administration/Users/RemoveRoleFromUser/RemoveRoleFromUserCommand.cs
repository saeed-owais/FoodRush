using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Users.RemoveRoleFromUser;

public sealed record RemoveRoleFromUserCommand(Guid UserId, Guid RoleId) : IRequest<Result>;