using FoodRush.Domain.Common;
using MediatR;

namespace FoodRush.Application.Features.Authentication.GetUserPrfile;

public sealed record GetUserPrfileQuery : IRequest<Result<GetUserProfileResponse>>;