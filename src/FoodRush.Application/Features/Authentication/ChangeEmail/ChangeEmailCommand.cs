using FoodRush.Domain.Common;
using MediatR;

namespace FoodRush.Application.Features.Authentication.ChangeEmail;

public sealed record ChangeEmailCommand(string NewEmail) : IRequest<Result>;