namespace FoodRush.Application.Abstractions.Notifications;

public interface IEmailTemplateRenderer
{
    string RenderVerifyEmail(string verificationLink);

    string RenderResetPassword(string resetLink);

    string RenderChangeEmail(string confirmationLink);
}
