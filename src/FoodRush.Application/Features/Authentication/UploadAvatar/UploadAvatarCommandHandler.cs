using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Abstractions.Storage;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Authentication.UploadAvatar;

internal sealed class UploadAvatarCommandHandler
(
    IApplicationDbContext dbContext,
    IFileStorageService storageService,
    IUserContext userContext
) : IRequestHandler<UploadAvatarCommand, Result<UploadAvatarResponse>>
{
    public async Task<Result<UploadAvatarResponse>> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .AsTracking()
            .FirstOrDefaultAsync(u => u.Id == userContext.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure<UploadAvatarResponse>(
                Error.NotFound("User.NotFound", $"User with ID {userContext.UserId} not found."));
        }

        string publicId = $"avatars/{user.Id}";

        var fileUrl = await storageService.UploadImageAsync(request.FileStream, publicId, cancellationToken);

        user.AvatarUrl = fileUrl;
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new UploadAvatarResponse(fileUrl));
    }
}
