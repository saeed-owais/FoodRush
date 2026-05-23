using FluentValidation;

namespace FoodRush.Application.Features.Authentication.Logout;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(500);
    }
}

