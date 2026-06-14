using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Authentication.Sessions.RevokeSession;

public sealed record RevokeSessionCommand(Guid SessionId)
    : IRequest<Result>;