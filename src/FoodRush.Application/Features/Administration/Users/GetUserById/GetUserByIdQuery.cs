using FoodRush.Domain.Common;
using MediatR;

namespace FoodRush.Application.Features.Administration.Users.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<Result<GetUserByIdResponse>>;