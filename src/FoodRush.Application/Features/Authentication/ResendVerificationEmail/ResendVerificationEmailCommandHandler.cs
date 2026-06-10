using FoodRush.Application.Abstractions.Authentication;
using FoodRush.Application.Abstractions.BackgroundJobs;
using FoodRush.Application.Abstractions.Notifications;
using FoodRush.Application.Abstractions.Persistence;
using FoodRush.Application.Common;
using FoodRush.Application.Common.Errors;
using FoodRush.Application.Common.Settings;
using FoodRush.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FoodRush.Application.Features.Authentication.ResendVerificationEmail;

internal sealed class ResendVerificationEmailCommandHandler
    (
    IApplicationDbContext dbContext,
    IEmailVerificationTokenProvider tokenProvider,
    IBackgroundJobService backgroundJobService,
    IOptions<FrontendSettings> options
    )
    : IRequestHandler<ResendVerificationEmailCommand, Result>
{
    private readonly FrontendSettings _frontendSettings = options.Value;

    public async Task<Result> Handle(ResendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        string normalizedEmail =
            request.Email.Trim().ToUpperInvariant();

        User? user = await dbContext.Users
            .AsNoTracking()
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
            $"{frontendSettings.EmailVerificationUrl}?token={Uri.EscapeDataString(token)}";


        backgroundJobService.Enqueue<IEmailService>(emailService => emailService.SendAsync(
            user.Email,
            "Verify Your Email",
            $"Please click the following link to verify your email: {verificationLink}"
            ));

        return Result.Success();
    }
}
