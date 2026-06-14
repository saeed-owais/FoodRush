using FluentValidation;

namespace FoodRush.Application.Features.Authentication.VerifyEmail
{
    internal sealed class VerifyEmailValidator : AbstractValidator<VerifyEmailCommand>
    {
        public VerifyEmailValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Token is required.");
        }
    }
}
