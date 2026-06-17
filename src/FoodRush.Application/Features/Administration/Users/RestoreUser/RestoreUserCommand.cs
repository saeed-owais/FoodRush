using FoodRush.Domain.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Users.RestoreUser;

public sealed record RestoreUserCommand(Guid UserId) : IRequest<Result>;