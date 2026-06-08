using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Authorization;
using FoodRush.Application.Common.Errors;
using FoodRush.Application.Common.Helpers;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FoodRush.Application.Features.Authentication.Register;

internal sealed class RegisterCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    public async Task<Result<RegisterResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {

        string normalizedEmail =
            request.Email.Trim().ToUpperInvariant();

        string? normalizedPhoneNumber =
            PhoneNumberNormalizer.Normalize(
                request.PhoneNumber!);

        bool emailExists = await dbContext.Users
            .AnyAsync(
                x => x.NormalizedEmail ==
                    normalizedEmail,
                cancellationToken);

        if (emailExists)
        {
            return Result.Failure<RegisterResponse>(
                Error.Conflict(
                    "Auth.EmailAlreadyExists",
                    "Email is already in use."));
        }

        bool phoneExists = await dbContext.Users
            .AnyAsync(
                x => x.NormalizedPhoneNumber ==
                    normalizedPhoneNumber,
                cancellationToken);

        if (phoneExists)
        {
            return Result.Failure<RegisterResponse>(
                Error.Conflict(
                    "Auth.PhoneAlreadyExists",
                    "Phone number is already in use."));
        }
        User user = new()
        {
            Email = request.Email,

            NormalizedEmail =
                normalizedEmail,

            PhoneNumber = request.PhoneNumber,

            NormalizedPhoneNumber =
                normalizedPhoneNumber,

            PasswordHash =
                passwordHasher.Hash(request.Password),

            DisplayName = request.DisplayName,

            IsActive = true,

            IsEmailVerified = false,

            IsPhoneVerified = false
        };

        var customerRole = await dbContext.Roles
            .FirstOrDefaultAsync(r => r.Code == Roles.Customer,
            cancellationToken);

        if (customerRole is null)
        {
            return Result.Failure<RegisterResponse>(
                Error.Failure(
                    "Auth.RoleNotFound",
                    "Customer role was not found."));
        }

        user.UserRoles.Add(new UserRole
        {
            RoleId =
                customerRole.Id
        });

        await dbContext.Users.AddAsync(
            user,
            cancellationToken);

        await dbContext.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            new RegisterResponse(user.Id));
    }
}