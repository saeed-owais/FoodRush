using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.Notifications;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FoodRush.Application.Features.Authentication.ResendVerificationEmail;

internal sealed class ResendVerificationEmailCommandHandler
    (
    IApplicationDbContext dbContext,
    IEmailVerificationTokenProvider tokenProvider,
    [FromKeyedServices("FakeEmailService")] IEmailService emailService
    )
    : IRequestHandler<ResendVerificationEmailCommand, Result>
{
    public async Task<Result> Handle(ResendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        string normalizedEmail =
            request.Email.Trim().ToUpperInvariant();

        User? user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

        if (user == null)
        {
            return Result.Success(); // To prevent email enumeration, we return success even if the user doesn't exist  
        }

        if (user.IsEmailVerified)
        {
            return Result.Failure(
                Error.Conflict("User.EmailAlreadyVerified", "User's email is already verified"));
        }

        var token = tokenProvider.GenerateToken(user);

        string verificationLink =
            $"https://localhost:3000/verify-email?token={Uri.EscapeDataString(token)}";

        await emailService.SendAsync(
            user.Email,
            "Verify Your Email",
            $"Please click the following link to verify your email: {verificationLink}",
            cancellationToken);

        return Result.Success();
    }
}
