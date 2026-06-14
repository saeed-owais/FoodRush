using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Authentication.ResetPassword;

public sealed record ResetPasswordCommand(
    string Token,
    string NewPassword,
    string ConfirmPassword) : IRequest<Result>;