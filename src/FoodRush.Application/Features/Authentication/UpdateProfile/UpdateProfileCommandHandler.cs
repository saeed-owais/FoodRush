using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Authentication.UpdateProfile;

internal sealed class UpdateProfileCommandHandler
(
    IApplicationDbContext dbContext,
    IUserContext userContext)
    : IRequestHandler<UpdateProfileCommand, Result>
{
    public async Task<Result> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.Id == userContext.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(
                Error.NotFound("User.NotFound", $"User with ID {userContext.UserId} not found."));
        }

        if (user.PhoneNumber != request.PhoneNumber)
        {
            user.IsPhoneVerified = false;
            user.PhoneNumber = request.PhoneNumber;
        }

        user.DisplayName = request.DisplayName;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
