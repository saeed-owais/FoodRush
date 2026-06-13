namespace FoodRush.Application.Common.Errors;

internal static class AuthErrors
{
    public static readonly Error InvalidCredentials =
        Error.Unauthorized(
            "Auth.InvalidCredentials",
            "Invalid email or password.");

    public static readonly Error InvalidRefreshToken =
        Error.Unauthorized(
            "Auth.InvalidRefreshToken",
            "The provided refresh token is invalid.");

    public static readonly Error InvalidPassword =
        Error.Validation("Auth.InvalidPassword", "The provided old password is incorrect.");


    public static readonly Error SamePassword =
        Error.Validation("User.SamePassword", "The new password must be different from the old password.");

    public static readonly Error AccountDisabled =
        Error.Unauthorized(
            "Auth.AccountDisabled",
            "Your account is disabled.");

    public static readonly Error InvalidEmailChangeToken =
        Error.Validation(
            "EmailChange.InvalidToken",
            "The email change token is invalid or expired.");

    public static readonly Error InvalidEmailVerifyToken =
        Error.Validation(
            "EmailVerify.InvalidToken",
            "The email verify token is invalid or expired.");

    public static readonly Error InvalidPasswordResetToken =
        Error.Validation(
            "PasswordReset.InvalidToken",
            "The password reset token is invalid or expired.");

    public static readonly Error SameEmail =
    Error.Validation(
        "Auth.Email.SameEmail",
        "The new email must be different from the current email.");

    public static readonly Error EmailNotVerified =
        Error.Conflict(
            "Auth.EmailNotVerified",
            "Email must be verified.");

    public static readonly Error UserInactive =
        Error.Unauthorized(
            "Auth.UserInactive",
            "The user account is inactive.");

    public static Error AccountLocked(double totalMinutes, int seconds)
    {
        return Error.Unauthorized(
            "Auth.AccountLocked",
            $"Your account is locked. Try again in {totalMinutes} minutes and {seconds} seconds.");
    }

    public static Error UserLockedOut(TimeSpan timeSpan)
    {
        return Error.Unauthorized(
            "Auth.UserLockedOut",
            $"The user account is locked until {DateTime.UtcNow.Add(timeSpan):u}.");
    }

}