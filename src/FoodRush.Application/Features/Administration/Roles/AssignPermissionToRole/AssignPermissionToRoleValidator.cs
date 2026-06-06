using FluentValidation;

namespace FoodRush.Application.Features.Administration.Roles.AssignPermissionToRole;

internal sealed class AssignPermissionToRoleValidator : AbstractValidator<AssignPermissionToRoleCommand>
{
    public AssignPermissionToRoleValidator()
    {
        RuleFor(x => x.RoleId).NotNull().NotEmpty();
        RuleFor(x => x.PermissionId).NotNull().NotEmpty();
    }
}
