namespace FoodRush.Infrastructure.Resilience;

internal static class PipelineNames
{
    public const string R2Upload = "r2-upload";
    public const string R2Remove = "r2-remove";

    public const string SendEmail = "send-email";
}
