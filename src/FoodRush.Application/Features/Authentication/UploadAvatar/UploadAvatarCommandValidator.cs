using FluentValidation;
using FoodRush.Application.Features.Authentication.UploadAvatar;

internal sealed class UploadAvatarCommandValidator
    : AbstractValidator<UploadAvatarCommand>
{
    private const long MaxSize = 5 * 1024 * 1024;

    private static readonly string[] AllowedTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    public UploadAvatarCommandValidator()
    {
        RuleFor(x => x.FileSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(MaxSize);

        RuleFor(x => x.ContentType)
            .Must(type => AllowedTypes.Contains(type))
            .WithMessage(
                "Only JPG, PNG and WEBP images are allowed.");
    }
}