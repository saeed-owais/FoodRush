using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Authentication.ConfirmEmailChange;

public sealed record ConfirmEmailChangeCommand(string Token) : IRequest<Result>;