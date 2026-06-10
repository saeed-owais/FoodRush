namespace FoodRush.Application.Common.Settings;

public sealed class FrontendSettings
{
    public const string SectionName = "Frontend";
    public string BaseUrl { get; set; } = default!;
    public string ResetPasswordUrl { get; set; } = default!;
    public string EmailVerificationUrl { get; set; } = default!;
}
