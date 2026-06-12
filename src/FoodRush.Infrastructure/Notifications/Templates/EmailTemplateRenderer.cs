using FoodRush.Application.Abstractions.Notifications;

namespace FoodRush.Infrastructure.Notifications.Templates;

internal sealed class EmailTemplateRenderer
    : IEmailTemplateRenderer
{
    public string RenderVerifyEmail(string verificationLink)
    {
        return $$"""
        Please verify your email by clicking the following link:    

        {{verificationLink}}

        If you did not create an account, you can safely ignore this email.
        """;
    }

    public string RenderResetPassword(string resetLink)
    {
        return $$"""
        Please click the following link to reset your password:

        {{resetLink}}

        If you did not request a password reset, you can safely ignore this email.
        """;
    }

    public string RenderChangeEmail(string confirmationLink)
    {
        return $$"""
        Please confirm your email change by clicking the following link:

        {{confirmationLink}}

        If you did not request this change, you can safely ignore this email.
        """;
    }
}