using FluentValidation;

namespace FoodRush.Application.Features.Authentication.ChangeEmail;

internal sealed class ChangeEmailCommandValidator : AbstractValidator<ChangeEmailCommand>
{
    public ChangeEmailCommandValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .EmailAddress();
    }
}
