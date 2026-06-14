using FluentValidation;

namespace FoodRush.Application.Features.Administration.Roles.RemovePermissionFromRole;

internal sealed class RemovePermissionFromRoleValidator : AbstractValidator<RemovePermissionFromRoleCommand>
{
    public RemovePermissionFromRoleValidator()
    {
        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("RoleId is required.");
        RuleFor(x => x.PermissionId)
            .NotEmpty().WithMessage("PermissionId is required.");
    }
}
