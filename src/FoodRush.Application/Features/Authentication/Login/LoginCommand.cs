using FoodRush.Domain.Common;
using MediatR;

namespace FoodRush.Application.Features.Authentication.Login;

public sealed record LoginCommand(
    string Email,
    string Password)
    : IRequest<Result<LoginResponse>>;

