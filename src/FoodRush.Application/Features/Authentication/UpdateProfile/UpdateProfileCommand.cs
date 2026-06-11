using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Authentication.UpdateProfile;

public sealed record UpdateProfileCommand(string? DisplayName, string? PhoneNumber)
    : IRequest<Result>;
