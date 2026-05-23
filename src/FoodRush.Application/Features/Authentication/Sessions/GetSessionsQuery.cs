using FoodRush.Application.Common;
using MediatR;

namespace FoodRush.Application.Features.Authentication.Sessions;

public sealed record GetSessionsQuery
    : IRequest<Result<IReadOnlyCollection<SessionResponse>>>;