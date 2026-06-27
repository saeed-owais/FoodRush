using FluentValidation;

namespace FoodRush.Application.Features.Restaurants.RegisterRestaurant;

internal sealed class RegisterRestaurantCommandValidator
    : AbstractValidator<RegisterRestaurantCommand>
{
    public RegisterRestaurantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180);

        RuleFor(x => x.DeliveryRadiusKm)
            .GreaterThanOrEqualTo(0);
    }
}