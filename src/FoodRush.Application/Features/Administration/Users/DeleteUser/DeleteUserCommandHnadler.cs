using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Administration.Users.DeleteUser;

internal sealed class DeleteUserCommandHnadler
    (IApplicationDbContext dbContext)
    : IRequestHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        User? user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(
                Error.NotFound("User.NotFound", $"User with ID {request.UserId} was not found"));
        }

        dbContext.Users.Remove(user);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
