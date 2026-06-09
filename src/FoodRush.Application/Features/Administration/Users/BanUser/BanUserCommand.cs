using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Users.BanUser;

public sealed record BanUserCommand(
    Guid UserId,
    DateTime? BanEndDate)
    : IRequest<Result>;