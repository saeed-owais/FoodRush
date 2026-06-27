using FoodRush.Domain.Common;
using MediatR;

namespace FoodRush.Application.Features.Authentication.Logout;

public sealed record LogoutCommand(string RefreshToken)
    : IRequest<Result>;

