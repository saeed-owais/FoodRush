using FluentValidation;

namespace FoodRush.Application.Features.Administration.Users.AssignPermissionToUser;

internal sealed class AssignPermissionToUserValidator : AbstractValidator<AssignPermissionToUserCommand>
{
    public AssignPermissionToUserValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.PermissionId)
            .NotEmpty().WithMessage("PermissionId is required.");
    }
}
