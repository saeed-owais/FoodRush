using FluentValidation;

namespace FoodRush.Application.Features.Administration.Roles.UpdateRole;

internal sealed class UpdateRoleValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Role ID is required.");
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name is required.")
            .MaximumLength(100).WithMessage("Role name cannot exceed 100 characters.");
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Role code is required.")
            .MaximumLength(100).WithMessage("Role code cannot exceed 100 characters.");
    }
}
