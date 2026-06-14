using FluentValidation;

namespace FoodRush.Application.Features.Authentication.ConfirmEmailChange
{
    internal sealed class ConfirmEmailChangeCommandValidator : AbstractValidator<ConfirmEmailChangeCommand>
    {
        public ConfirmEmailChangeCommandValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty();
        }
    }
}
