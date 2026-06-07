using FluentValidation;

namespace FoodRush.Application.Features.Administration.Permissions.DeletePermission;

internal sealed class DeletePermissionValidator : AbstractValidator<DeletePermissionCommand>
{
    public DeletePermissionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Permission ID is required.");
    }
}
