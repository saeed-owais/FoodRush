using FluentValidation;

namespace FoodRush.Application.Features.Administration.Restaurants.Commands.SuspendRestaurant;

public sealed class SuspendRestaurantCommandValidator : AbstractValidator<SuspendRestaurantCommand>
{
    public SuspendRestaurantCommandValidator()
    {
        RuleFor(x => x.RestaurantId)
            .NotEmpty().WithMessage("Restaurant ID is required.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason for suspension is required.")
            .MaximumLength(500).WithMessage("Reason for suspension cannot exceed 500 characters.");
    }
}
