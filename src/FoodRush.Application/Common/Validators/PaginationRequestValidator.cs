using FluentValidation;
using FoodRush.Application.Common.Models;

namespace FoodRush.Application.Common.Validators;

public class PaginationRequestValidator<T>
    : AbstractValidator<T>
    where T : PaginationRequest
{
    protected PaginationRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);
    }
}