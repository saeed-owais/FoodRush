using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Authentication.ResendVerificationEmail;

public sealed record ResendVerificationEmailCommand(string Email) : IRequest<Result>;