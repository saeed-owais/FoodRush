using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Authentication.Sessions.LogoutAllSessions;

public sealed record LogoutAllSessionsCommand
    : IRequest<Result>;