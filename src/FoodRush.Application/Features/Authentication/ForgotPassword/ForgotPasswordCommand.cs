using FoodRush.Domain.Common;
using MediatR;

namespace FoodRush.Application.Features.Authentication.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<Result>;