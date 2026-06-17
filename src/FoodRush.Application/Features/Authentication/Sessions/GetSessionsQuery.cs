using FoodRush.Domain.Common;
using MediatR;

namespace FoodRush.Application.Features.Authentication.Sessions;

public sealed record GetSessionsQuery
    : IRequest<Result<IReadOnlyCollection<SessionResponse>>>;