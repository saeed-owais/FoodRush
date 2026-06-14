using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Users.UnBanUser;

public sealed record UnBanUserCommand(Guid UserId) : IRequest<Result>;