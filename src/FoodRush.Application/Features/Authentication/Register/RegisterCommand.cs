using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Authentication.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string DisplayName,
    string? PhoneNumber)
    : IRequest<Result<RegisterResponse>>;

