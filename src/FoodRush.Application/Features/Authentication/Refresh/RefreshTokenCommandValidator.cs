using FluentValidation;

namespace FoodRush.Application.Features.Authentication.Refresh;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(500);
    }
}


