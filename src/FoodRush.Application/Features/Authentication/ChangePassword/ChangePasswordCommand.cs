using FoodRush.Domain.Common;
using MediatR;

namespace FoodRush.Application.Features.Authentication.ChangePassword;

public sealed record ChangePasswordCommand
(
    string OldPassword,
    string NewPassword,
    string ConfirmNewPassword
) : IRequest<Result>;