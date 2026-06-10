using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Authentication.VerifyEmail;

public sealed record VerifyEmailCommand(string Token) : IRequest<Result>;