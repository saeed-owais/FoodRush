using FoodRush.Domain.Common;
using MediatR;

namespace FoodRush.Application.Features.Authentication.Refresh;

public sealed record RefreshTokenCommand(
    string RefreshToken)
    : IRequest<Result<RefreshTokenResponse>>;

