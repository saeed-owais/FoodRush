using FluentValidation;

namespace FoodRush.Application.Features.Restaurants.UploadDocument;

public sealed class UploadDocumentCommandValidator
    : AbstractValidator<UploadDocumentCommand>
{
    private const long MaxFileSizeBytes = 50 * 1024 * 1024;

    public UploadDocumentCommandValidator()
    {
        RuleFor(x => x.RestaurantId)
            .NotEmpty();

        RuleFor(x => x.FileStream)
            .NotNull();

        RuleFor(x => x.FileName)
            .NotEmpty();

        RuleFor(x => x.ContentType)
            .NotEmpty();

        RuleFor(x => x.FileSize)
            .GreaterThan(0)
            .WithMessage("File cannot be empty.");

        RuleFor(x => x.FileSize)
            .LessThanOrEqualTo(MaxFileSizeBytes)
            .WithMessage("File size cannot exceed 50 MB.");

        RuleFor(x => Path.GetExtension(x.FileName))
            .Equal(".pdf")
            .WithMessage("Only PDF files are allowed.");

        RuleFor(x => x.ContentType)
            .Equal("application/pdf")
            .WithMessage("Only PDF files are allowed.");
    }
}