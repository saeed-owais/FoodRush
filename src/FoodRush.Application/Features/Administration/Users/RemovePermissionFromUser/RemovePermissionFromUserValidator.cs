using FluentValidation;

namespace FoodRush.Application.Features.Administration.Users.RemovePermissionFromUser;

internal sealed class RemovePermissionFromUserValidator : AbstractValidator<RemovePermissionFromUserCommand>
{
    public RemovePermissionFromUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");
        RuleFor(x => x.PermissionId)
            .NotEmpty().WithMessage("PermissionId is required.");
    }
}
