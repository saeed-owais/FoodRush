using FluentValidation;

namespace FoodRush.Application.Features.Administration.Roles.DeleteRole;

internal class DeleteRoleValidator : AbstractValidator<DeleteRoleCommand>
{
    public DeleteRoleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Role Id is required.");
    }
}
